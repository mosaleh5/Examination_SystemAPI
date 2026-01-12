using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.Common;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.ExamAttemptServices
{
    public class ExamAttemptServices : GenericServices<ExamAttempt>, IExamAttemptServices
    {
        public ExamAttemptServices(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        public async Task<Result<ExamAttemptDto>> GetAttemptByIdAsync(Guid attemptId)
        {
            var attemptSpec = new ExamAttemptSpecifications(s => s.Id == attemptId);
            var attempt = await _unitOfWork.Repository<ExamAttempt>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .FirstOrDefaultAsync();

            if (attempt == null)
            {
                return Result<ExamAttemptDto>.Failure(
                    ErrorCode.NotFound,
                    $"Exam attempt with ID {attemptId} not found");
            }

            var attemptDto = _mapper.Map<ExamAttemptDto>(attempt);
            return Result<ExamAttemptDto>.Success(attemptDto);
        }

        public async Task<Result<ExamToAttemptDto>> StartExamAsync(Guid examId, Guid studentId)
        {
            // Validate student enrollment
            if (!await IsStudentEnrolledInExamAsync(examId, studentId))
            {
                return Result<ExamToAttemptDto>.Failure(
                    ErrorCode.Forbidden,
                    $"Student {studentId} is not assigned to exam {examId}");
            }

            // Get exam details
            var exam = await GetExamByIdAsync(examId);
            if (exam == null)
            {
                return Result<ExamToAttemptDto>.Failure(
                    ErrorCode.NotFound,
                    $"Exam with ID {examId} not found");
            }

            // Validate exam availability
            var validationResult = ValidateExamAvailability(exam, studentId);
            if (!validationResult.IsSuccess)
            {
                return Result<ExamToAttemptDto>.Failure(validationResult.Error, validationResult.ErrorMessage);
            }

            // Create and save exam attempt
            var attemptExamStart = new ExamAttempt
            {
                ExamId = exam.Id,
                StudentId = studentId,
                StartedAt = DateTime.UtcNow,
            };

            await _unitOfWork.Repository<ExamAttempt>().AddAsync(attemptExamStart);
            var rowsAffected = await _unitOfWork.CompleteAsync();

            if (rowsAffected < 1)
            {
                return Result<ExamToAttemptDto>.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to start exam. Database error occurred.");
            }

            var examToAttempt = _mapper.Map<ExamToAttemptDto>(exam);
            examToAttempt.StartedAt = attemptExamStart.StartedAt;
            examToAttempt.ID = attemptExamStart.Id;

            return Result<ExamToAttemptDto>.Success(examToAttempt);
        }

        public async Task<Result<ExamAttemptDto>> SubmitExamAsync(Guid attemptId, List<SubmitAnswerDto> answers)
        {
            // Validate input
            if (answers == null || !answers.Any())
            {
                return Result<ExamAttemptDto>.Failure(
                    ErrorCode.ValidationError,
                    "Answers cannot be null or empty");
            }

            // Get attempt with related data
            var attempt = await GetAttemptWithDetailsAsync(attemptId);
            if (attempt == null)
            {
                return Result<ExamAttemptDto>.Failure(
                    ErrorCode.NotFound,
                    $"Exam attempt with ID {attemptId} not found");
            }

            // Validate submission
            var validationResult = ValidateExamSubmission(attempt, answers);
            if (!validationResult.IsSuccess)
            {
                return Result<ExamAttemptDto>.Failure(validationResult.Error, validationResult.ErrorMessage);
            }

            // Evaluate answers and update attempt
            var studentAnswers = await EvaluateExamAsync(attempt, answers);
            UpdateAttemptWithResults(attempt);

            // Save changes
            await _unitOfWork.Repository<ExamAttempt>().UpdatePartialAsync(attempt);
            await SaveStudentAnswersAsync(studentAnswers);

            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
            {
                return Result<ExamAttemptDto>.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to submit exam. Database error occurred.");
            }

            // Return updated attempt
            var reloadedAttempt = await GetAttemptWithDetailsAsync(attemptId);
            var attemptDto = _mapper.Map<ExamAttemptDto>(reloadedAttempt);
            attemptDto.PassingPercentage = attempt.Exam.PassingPercentage;
            
            return Result<ExamAttemptDto>.Success(attemptDto);
        }

        public async Task<Result<IEnumerable<ExamAttemptDto>>> GetStudentAttemptsForInstructorAsync(Guid instructorId)
        {
            var instructorExamIds = await GetInstructorExamIdsAsync(instructorId);

            var attemptSpec = new ExamAttemptSpecifications();
            var attemptsList = await _unitOfWork.Repository<ExamAttempt>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .Where(s => instructorExamIds.Contains(s.ExamId))
                .ProjectTo<ExamAttemptDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!attemptsList.Any())
            {
                return Result<IEnumerable<ExamAttemptDto>>.Failure(
                    ErrorCode.NotFound,
                    $"No student attempts found for instructor {instructorId}");
            }

            return Result<IEnumerable<ExamAttemptDto>>.Success(attemptsList);
        }

        public async Task<Result<IEnumerable<ExamAttemptDto>>> GetStudentAttemptsAsync(Guid instructorId, Guid studentId)
        {
            var instructorExamIds = await GetInstructorExamIdsAsync(instructorId);

            var attemptSpec = new ExamAttemptSpecifications();
            var attemptsList = await _unitOfWork.Repository<ExamAttempt>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .Where(s => instructorExamIds.Contains(s.ExamId) && s.StudentId == studentId)
                .ProjectTo<ExamAttemptDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!attemptsList.Any())
            {
                return Result<IEnumerable<ExamAttemptDto>>.Failure(
                    ErrorCode.NotFound,
                    $"No attempts found for student {studentId} in instructor {instructorId}'s exams");
            }

            return Result<IEnumerable<ExamAttemptDto>>.Success(attemptsList);
        }

        public async Task<Result<IEnumerable<ExamAttemptDto>>> GetStudentAttemptsForStudentAsync(Guid studentId)
        {
            var attemptSpec = new ExamAttemptSpecifications();
            var attemptsList = await _unitOfWork.Repository<ExamAttempt>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .Where(s => s.StudentId == studentId)
                .ProjectTo<ExamAttemptDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!attemptsList.Any())
            {
                return Result<IEnumerable<ExamAttemptDto>>.Failure(
                    ErrorCode.NotFound,
                    $"No attempts found for student {studentId}");
            }

            return Result<IEnumerable<ExamAttemptDto>>.Success(attemptsList);
        }

        #region Private Helper Methods

        private async Task<Exam?> GetExamByIdAsync(Guid examId)
        {
            var examSpecifications = new ExamSpecifications(e => e.Id == examId);
            return await _unitOfWork.Repository<Exam>()
                .GetByIdWithSpecification(examSpecifications)
                .FirstOrDefaultAsync();
        }

        private async Task<ExamAttempt?> GetAttemptWithDetailsAsync(Guid attemptId)
        {
            var attemptSpec = new ExamAttemptSpecifications(s => s.Id == attemptId);
            return await _unitOfWork.Repository<ExamAttempt>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .FirstOrDefaultAsync();
        }

        private async Task<List<Guid>> GetInstructorExamIdsAsync(Guid instructorId)
        {
            var instructorExamsSpec = new ExamSpecifications(e => e.InstructorId == instructorId);
            return await _unitOfWork.Repository<Exam>()
                .GetAllWithSpecificationAsync(instructorExamsSpec)
                .Select(e => e.Id)
                .ToListAsync();
        }

        private async Task<bool> IsExamCompletedAsync(Guid examId, Guid studentId)
        {
            return await _unitOfWork.Repository<ExamAttempt>()
                .GetAll()
                .AnyAsync(at => at.ExamId == examId && at.StudentId == studentId && at.IsCompleted);
        }

        private async Task<bool> IsStudentEnrolledInExamAsync(Guid examId, Guid studentId)
        {
            return await _unitOfWork.Repository<ExamAssignment>()
                .GetAll()
                .AnyAsync(e => e.ExamId == examId && e.StudentId == studentId);
        }

        private Result<object> ValidateExamAvailability(Exam exam, Guid studentId)
        {
            if (!exam.IsActive)
            {
                return Result<object>.Failure(
                    ErrorCode.Forbidden,
                    $"Exam {exam.Id} is not currently active");
            }

            if (exam.ExamType == ExamType.Final && IsExamCompletedAsync(exam.Id, studentId).Result)
            {
                return Result<object>.Failure(
                    ErrorCode.Conflict,
                    $"You have already completed this final exam {exam.Id}");
            }

            return Result<object>.Success(null);
        }

        private Result<object> ValidateExamSubmission(ExamAttempt attempt, List<SubmitAnswerDto> answers)
        {
            if (attempt.IsCompleted)
            {
                return Result<object>.Failure(
                    ErrorCode.Conflict,
                    $"Exam attempt {attempt.Id} has already been submitted");
            }

            var durationInMinutes = (DateTime.UtcNow - attempt.StartedAt).TotalMinutes;
            if (durationInMinutes > attempt.Exam.DurationInMinutes)
            {
                return Result<object>.Failure(
                    ErrorCode.BadRequest,
                    $"Submission time exceeded. Exam duration: {attempt.Exam.DurationInMinutes} minutes, " +
                    $"Actual time: {durationInMinutes:F2} minutes");
            }

            if (answers.Count != attempt.Exam.QuestionsCount)
            {
                return Result<object>.Failure(
                    ErrorCode.ValidationError,
                    $"Invalid number of answers. Expected: {attempt.Exam.QuestionsCount}, Received: {answers.Count}");
            }

            return Result<object>.Success(null);
        }

        private void UpdateAttemptWithResults(ExamAttempt attempt)
        {
            attempt.Percentage = attempt.MaxScore > 0 
                ? ((double)attempt.Score / attempt.MaxScore) * 100 
                : 0;
            attempt.IsSucceed = attempt.Percentage >= attempt.Exam.PassingPercentage;
            attempt.IsCompleted = true;
            attempt.SubmittedAt = DateTime.UtcNow;
        }

        private async Task SaveStudentAnswersAsync(List<StudentAnswer> studentAnswers)
        {
            foreach (var studentAnswer in studentAnswers)
            {
                await _unitOfWork.Repository<StudentAnswer>().AddAsync(studentAnswer);
            }
        }

        private async Task<List<StudentAnswer>> EvaluateExamAsync(ExamAttempt attempt, List<SubmitAnswerDto> answers)
        {
            attempt.Score = 0;
            attempt.MaxScore = 0;

            var studentAnswers = new List<StudentAnswer>();
            var questionIds = answers.Select(a => a.QuestionId).ToList();
            var questions = await GetQuestionsByIdsAsync(questionIds);

            foreach (var answer in answers)
            {
                var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null) continue;

                var studentAnswer = CreateStudentAnswer(answer, question, attempt.Id);
                studentAnswers.Add(studentAnswer);

                if (studentAnswer.IsCorrect)
                {
                    attempt.Score += question.mark;
                }

                attempt.MaxScore += question.mark;
            }

            return studentAnswers;
        }

        private async Task<List<Question>> GetQuestionsByIdsAsync(List<Guid> questionIds)
        {
            var questionSpecifications = new QuestionSpecifications(q => questionIds.Contains(q.Id));
            return await _unitOfWork.Repository<Question>()
                .GetAllWithSpecificationAsync(questionSpecifications)
                .ToListAsync();
        }

        private StudentAnswer CreateStudentAnswer(SubmitAnswerDto answer, Question question, Guid attemptId)
        {
            var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect);
            var isCorrect = correctChoice != null && correctChoice.Id == answer.ChoiceId;

            return new StudentAnswer
            {
                QuestionId = question.Id,
                SelectedChoiceId = answer.ChoiceId,
                AttemptId = attemptId,
                IsCorrect = isCorrect
            };
        }

        #endregion
    }
}
