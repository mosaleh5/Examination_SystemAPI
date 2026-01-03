using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.ExamAttemptServices
{
    public class ExamAttemptServices : GenericServices<ExamAttempt, int> , IExamAttemptServices
    {
        public ExamAttemptServices(IUnitOfWork unitOfWork , IMapper mapper):base(unitOfWork, mapper) { }
          
        public Task<ExamAttemptDto> GetAttemptByIdAsync(int attemptId)
        {
            throw new NotImplementedException();
        }

    

        
        public async Task<ExamToAttemptDto> StartExamAsync(int examId, string studentId)
        {

            if (!await IsStudentAlreadyEnrolledToThisExam(examId, studentId))
            {
                throw new InvalidOperationException($"Student {studentId} is not assigned to exam {examId}.");
            }
            var examSpecifications = new ExamSpecifications(e => e.Id == examId);
            var exam = (Exam)await _unitOfWork.Repository<Exam, int>().GetByIdWithSpecification(examSpecifications);

            if (exam == null)
            {
                throw new KeyNotFoundException($"Exam with ID {examId} not found.");
            }



            if (!exam.IsActive)
            {
                throw new InvalidOperationException($"You do not have access to enroll to exam {examId}.");
            }

            if (exam.ExamType == ExamType.Final && await IsCompeleted(exam.Id, studentId))
            {
                throw new InvalidOperationException($"You completed this final exam {examId}.");
            }
            var AttemptExamStart = new ExamAttempt
            {
                ExamId = exam.Id,
                StudentId = studentId,
                StartedAt = DateTime.UtcNow,

            };

            await _unitOfWork.Repository<ExamAttempt, int>().AddAsync(AttemptExamStart);
            await _unitOfWork.CompleteAsync();
            var examToAttempt = _mapper.Map<ExamToAttemptDto>(exam);
            examToAttempt.StartedAt = AttemptExamStart.StartedAt;
            examToAttempt.ID = AttemptExamStart.Id;
            return examToAttempt;
        }

        public async Task<bool> IsCompeleted(int examId, string studentId)
        {
            return await _unitOfWork.Repository<ExamAttempt, int>().GetAll()
                .AnyAsync(at => at.ExamId == examId && at.StudentId == studentId && at.IsCompleted);
        }
        public async Task<ExamAttemptDto> SubmitExamAsync(int attemptId, List<SubmitAnswerDto> answers)
        {
            if (attemptId <= 0)
                throw new ArgumentException("Invalid attempt ID.", nameof(attemptId));

            if (answers == null || !answers.Any())
                throw new ArgumentException("Answers cannot be null or empty.", nameof(answers));

            var examSpecification = new ExamAttemptSpecifications(s => s.Id ==  attemptId);
            var attempt = await _unitOfWork.Repository<ExamAttempt, int>().GetAllWithSpecificationAsync(examSpecification).FirstOrDefaultAsync();
            
            if (attempt == null)
                throw new KeyNotFoundException($"Exam attempt with ID {attemptId} not found.");

            if (attempt.IsCompleted)
                throw new InvalidOperationException($"Exam attempt {attemptId} has already been submitted.");

            var durationInMinuteToFinish = DateTime.UtcNow - attempt.StartedAt;
            if (durationInMinuteToFinish.TotalMinutes > attempt.Exam.DurationInMinutes)
            {
                throw new InvalidOperationException(
                    $"Submission time exceeded. Exam duration: {attempt.Exam.DurationInMinutes} minutes, " +
                    $"Actual time: {durationInMinuteToFinish.TotalMinutes:F2} minutes.");
            }
            
            if (answers.Count != attempt.Exam.QuestionsCount)
            {
                throw new InvalidOperationException(
                    $"Invalid number of answers. Expected: {attempt.Exam.QuestionsCount}, Received: {answers.Count}.");
            }

            var studentAnswers = await EvaluateExamAsync(attempt, answers);

            attempt.Percentage = attempt.MaxScore > 0 ? ((double)attempt.Score / attempt.MaxScore) * 100 : 0;
            attempt.IsSucceed = attempt.Percentage >= attempt.Exam.PassingPercentage;
            attempt.IsCompleted = true;
            attempt.SubmittedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<ExamAttempt, int>().UpdatePartialAsync(attempt);
            
            // Add student answers separately
            foreach (var studentAnswer in studentAnswers)
            {
                await _unitOfWork.Repository<StudentAnswer, int>().AddAsync(studentAnswer);
            }
            
            await _unitOfWork.CompleteAsync();

            // Reload attempt with student answers for mapping
            var reloadedAttempt = await _unitOfWork.Repository<ExamAttempt, int>().GetAllWithSpecificationAsync(examSpecification).FirstOrDefaultAsync();
            var AttemptDto =  _mapper.Map<ExamAttemptDto>(reloadedAttempt);
            return AttemptDto;
        }
        private async Task<List<StudentAnswer>> EvaluateExamAsync(ExamAttempt attempt, List<SubmitAnswerDto> answers)
        {
            attempt.Score = 0;
            attempt.MaxScore = 0;
            
            var studentAnswers = new List<StudentAnswer>();
            
            var questionIds = answers.Select(a => a.QuestionId).ToList();
            var questionSpecifications = new QuestionSpecifications(q => questionIds.Contains(q.Id));
            var questions = await _unitOfWork.Repository<Question, int>().GetAllWithSpecificationAsync(questionSpecifications).ToListAsync();
            
            foreach (var answer in answers)
            {
                var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question != null)
                {
                    var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect);
                    var isCorrect = correctChoice != null && correctChoice.Id == answer.choiceId;
                    
                    var studentAnswer = new StudentAnswer
                    {
                        QuestionId = question.Id,
                        SelectedChoiceId = answer.choiceId > 0 ? answer.choiceId : null,
                        AttemptId = attempt.Id,
                        IsCorrect = isCorrect
                    };
                    
                    if (isCorrect)
                    {
                        attempt.Score += question.mark;
                    }
                    
                    studentAnswers.Add(studentAnswer);
                    attempt.MaxScore += question.mark;
                }
            }
       
            return studentAnswers;
        }
        private async Task<bool> IsStudentAlreadyEnrolledToThisExam(int examId, string studentId)
        {
            return await _unitOfWork.Repository<ExamAssignment, int>().GetAll()
                .AnyAsync(e => e.ExamId == examId && e.StudentId == studentId);
        }
        public async Task<IEnumerable<ExamAttemptDto>> GetStudentAttemptsForInstructorAsync(string instructorId)
        {
            var instructorExamsSpec = new ExamSpecifications(e => e.InstructorId == instructorId);
            var instructorExams = await _unitOfWork.Repository<Exam, int>()
                .GetAllWithSpecificationAsync(instructorExamsSpec).Select(e => e.Id).ToListAsync();
            var attemptSpec = new ExamAttemptSpecifications();
            var attemptsList = await _unitOfWork.Repository<ExamAttempt, int>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .Where(s => instructorExams.Contains(s.ExamId))
                .ProjectTo<ExamAttemptDto>(_mapper.ConfigurationProvider).ToListAsync();
            return attemptsList;
        }
        public async Task<IEnumerable<ExamAttemptDto>> GetStudentAttemptsAsync(string instructorId, string studentId)
        {
            var instructorExamsSpec = new ExamSpecifications(e => e.InstructorId == instructorId);
            var instructorExams = await _unitOfWork.Repository<Exam, int>()
                .GetAllWithSpecificationAsync(instructorExamsSpec).Select(e => e.Id).ToListAsync();
            var attemptSpec = new ExamAttemptSpecifications();
            var attemptsList = await _unitOfWork.Repository<ExamAttempt, int>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .Where(s => instructorExams.Contains(s.ExamId) && s.StudentId == studentId)
                .ProjectTo<ExamAttemptDto>(_mapper.ConfigurationProvider).ToListAsync();

            return attemptsList;
        }

        public async Task<IEnumerable<ExamAttemptDto>> GetStudentAttemptsForStudentAsync(string StudentId)
        {
                  
            var attemptSpec = new ExamAttemptSpecifications();
            var attemptsList = await _unitOfWork.Repository<ExamAttempt, int>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .Where(s => s.StudentId == StudentId)
                .ProjectTo<ExamAttemptDto>(_mapper.ConfigurationProvider).ToListAsync();

            return attemptsList;
        }
    }
}
