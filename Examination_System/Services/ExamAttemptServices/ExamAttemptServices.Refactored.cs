/*using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.Common;
using Examination_System.Common.Constants;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.ExamAttemptServices
{
    /// <summary>
    /// Service for managing exam attempts including starting, submitting, and retrieving attempts
    /// </summary>
    public class ExamAttemptServices : GenericServices<ExamAttempt>, IExamAttemptServices
    {
        public ExamAttemptServices(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        /// <summary>
        /// Retrieves an exam attempt by its ID
        /// </summary>
        public async Task<Result<ExamAttemptDto>> GetAttemptByIdAsync(Guid attemptId)
        {
            var attempt = await GetAttemptWithDetailsAsync(attemptId);

            if (attempt == null)
            {
                return Result<ExamAttemptDto>.Failure(
                    ErrorCode.NotFound,
                    string.Format(ErrorMessages.ExamAttemptNotFound, attemptId));
            }

            var attemptDto = _mapper.Map<ExamAttemptDto>(attempt);
            return Result<ExamAttemptDto>.Success(attemptDto);
        }

        /// <summary>
        /// Starts a new exam attempt for a student
        /// </summary>
        public async Task<Result<ExamToAttemptDto>> StartExamAsync(Guid examId, Guid studentId)
        {
            // Validate student enrollment
            if (!await IsStudentEnrolledInExamAsync(examId, studentId))
            {
                return Result<ExamToAttemptDto>.Failure(
                    ErrorCode.Forbidden,
                    string.Format(ErrorMessages.StudentNotEnrolled, studentId, examId));
            }

            // Get and validate exam
            var exam = await GetExamByIdAsync(examId);
            if (exam == null)
            {
                return Result<ExamToAttemptDto>.Failure(
                    ErrorCode.NotFound,
                    string.Format(ErrorMessages.ExamNotFound, examId));
            }

            var validationResult = await ValidateExamAvailabilityAsync(exam, studentId);
            if (!validationResult.IsSuccess)
            {
                return Result<ExamToAttemptDto>.Failure(validationResult.Error, validationResult.ErrorMessage);
            }

            // Create and save attempt
            var attempt = await CreateAndSaveAttemptAsync(exam.Id, studentId);
            if (attempt == null)
            {
                return Result<ExamToAttemptDto>.Failure(
                    ErrorCode.DatabaseError,
                    ErrorMessages.ExamStartFailed);
            }

            var examToAttempt = MapExamToAttemptDto(exam, attempt);
            return Result<ExamToAttemptDto>.Success(examToAttempt);
        }

        /// <summary>
        /// Submits an exam attempt with student answers
        /// </summary>
        public async Task<Result<ExamAttemptDto>> SubmitExamAsync(Guid attemptId, List<SubmitAnswerDto> answers)
        {
            // Validate input
            if (answers == null || !answers.Any())
            {
                return Result<ExamAttemptDto>.Failure(
                    ErrorCode.ValidationError,
                    ErrorMessages.AnswersRequired);
            }

            // Get and validate attempt
            var attempt = await GetAttemptWithDetailsAsync(attemptId);
            if (attempt == null)
            {
                return Result<ExamAttemptDto>.Failure(
                    ErrorCode.NotFound,
                    string.Format(ErrorMessages.ExamAttemptNotFound, attemptId));
            }

            var validationResult = ValidateExamSubmission(attempt, answers);
            if (!validationResult.IsSuccess)
            {
                return Result<ExamAttemptDto>.Failure(validationResult.Error, validationResult.ErrorMessage);
            }

            // Process submission
            var studentAnswers = await EvaluateExamAsync(attempt, answers);
            UpdateAttemptWithResults(attempt);

            // Save changes
            var saveResult = await SaveExamSubmissionAsync(attempt, studentAnswers);
            if (!saveResult)
            {
                return Result<ExamAttemptDto>.Failure(
                    ErrorCode.DatabaseError,
                    ErrorMessages.ExamSubmitFailed);
            }

            // Return updated attempt
            var submittedAttempt = await GetAttemptWithDetailsAsync(attemptId);
            var attemptDto = _mapper.Map<ExamAttemptDto>(submittedAttempt);
            attemptDto.PassingPercentage = attempt.Exam.PassingPercentage;

            return Result<ExamAttemptDto>.Success(attemptDto);
        }

        /// <summary>
        /// Gets all exam attempts for exams created by a specific instructor
        /// </summary>
        public async Task<Result<IEnumerable<ExamAttemptDto>>> GetStudentAttemptsForInstructorAsync(Guid instructorId)
        {
            var instructorExamIds = await GetInstructorExamIdsAsync(instructorId);
            var attemptsList = await GetAttemptsByExamIdsAsync(instructorExamIds);

            if (!attemptsList.Any())
            {
                return Result<IEnumerable<ExamAttemptDto>>.Failure(
                    ErrorCode.NotFound,
                    string.Format(ErrorMessages.NoStudentAttemptsFound, instructorId));
            }

            return Result<IEnumerable<ExamAttemptDto>>.Success(attemptsList);
        }

        /// <summary>
        /// Gets all exam attempts for a specific student in an instructor's exams
        /// </summary>
        public async Task<Result<IEnumerable<ExamAttemptDto>>> GetStudentAttemptsAsync(Guid instructorId, Guid studentId)
        {
            var instructorExamIds = await GetInstructorExamIdsAsync(instructorId);
            var attemptsList = await GetAttemptsByExamIdsAndStudentAsync(instructorExamIds, studentId);

            if (!attemptsList.Any())
            {
                return Result<IEnumerable<ExamAttemptDto>>.Failure(
                    ErrorCode.NotFound,
                    string.Format("No attempts found for student {0} in instructor {1}'s exams", studentId, instructorId));
            }

            return Result<IEnumerable<ExamAttemptDto>>.Success(attemptsList);
        }

        /// <summary>
        /// Gets all exam attempts for a specific student
        /// </summary>
        public async Task<Result<IEnumerable<ExamAttemptDto>>> GetStudentAttemptsForStudentAsync(Guid studentId)
        {
            var attemptsList = await GetAttemptsByStudentIdAsync(studentId);

            if (!attemptsList.Any())
            {
                return Result<IEnumerable<ExamAttemptDto>>.Failure(
                    ErrorCode.NotFound,
                    string.Format(ErrorMessages.NoAttemptsFound, $"student {studentId}"));
            }

            return Result<IEnumerable<ExamAttemptDto>>.Success(attemptsList);
        }

        #region Validation Methods

        private async Task<Result<object>> ValidateExamAvailabilityAsync(Exam exam, Guid studentId)
        {
            if (!exam.IsActive)
            {
                return Result<object>.Failure(
                    ErrorCode.Forbidden,
                    string.Format(ErrorMessages.ExamNotActive, exam.Id));
            }

            if (exam.ExamType == ExamType.Final && await IsExamCompletedAsync(exam.Id, studentId))
            {
                return Result<object>.Failure(
                    ErrorCode.Conflict,
                    string.Format(ErrorMessages.ExamAlreadyCompleted, exam.Id));
            }

            return Result<object>.Success(null);
        }

        private Result<object> ValidateExamSubmission(ExamAttempt attempt, List<SubmitAnswerDto> answers)
        {
            if (attempt.IsCompleted)
            {
                return Result<object>.Failure(
                    ErrorCode.Conflict,
                    string.Format(ErrorMessages.ExamAttemptAlreadySubmitted, attempt.Id));
            }

            var durationInMinutes = (DateTime.UtcNow - attempt.StartedAt).TotalMinutes;
            if (durationInMinutes > attempt.Exam.DurationInMinutes)
            {
                return Result<object>.Failure(
                    ErrorCode.BadRequest,
                    string.Format(ErrorMessages.ExamTimeExceeded, attempt.Exam.DurationInMinutes, durationInMinutes));
            }

            if (answers.Count != attempt.Exam.QuestionsCount)
            {
                return Result<object>.Failure(
                    ErrorCode.ValidationError,
                    string.Format(ErrorMessages.InvalidAnswerCount, attempt.Exam.QuestionsCount, answers.Count));
            }

            return Result<object>.Success(null);
        }

        #endregion

        #region Data Retrieval Methods

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

        private async Task<List<ExamAttemptDto>> GetAttemptsByExamIdsAsync(List<Guid> examIds)
        {
            var attemptSpec = new ExamAttemptSpecifications();
            return await _unitOfWork.Repository<ExamAttempt>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .Where(s => examIds.Contains(s.ExamId))
                .ProjectTo<ExamAttemptDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        private async Task<List<ExamAttemptDto>> GetAttemptsByExamIdsAndStudentAsync(List<Guid> examIds, Guid studentId)
        {
            var attemptSpec = new ExamAttemptSpecifications();
            return await _unitOfWork.Repository<ExamAttempt>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .Where(s => examIds.Contains(s.ExamId) && s.StudentId == studentId)
                .ProjectTo<ExamAttemptDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        private async Task<List<ExamAttemptDto>> GetAttemptsByStudentIdAsync(Guid studentId)
        {
            var attemptSpec = new ExamAttemptSpecifications();
            return await _unitOfWork.Repository<ExamAttempt>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .Where(s => s.StudentId == studentId)
                .ProjectTo<ExamAttemptDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        private async Task<List<Question>> GetQuestionsByIdsAsync(List<Guid> questionIds)
        {
            var questionSpecifications = new QuestionSpecifications(q => questionIds.Contains(q.Id));
            return await _unitOfWork.Repository<Question>()
                .GetAllWithSpecificationAsync(questionSpecifications)
                .ToListAsync();
        }

        #endregion

        #region Business Logic Methods

        private async Task<ExamAttempt?> CreateAndSaveAttemptAsync(Guid examId, Guid studentId)
        {
            var attempt = new ExamAttempt
            {
                ExamId = examId,
                StudentId = studentId,
                StartedAt = DateTime.UtcNow,
            };

            await _unitOfWork.Repository<ExamAttempt>().AddAsync(attempt);
            var rowsAffected = await _unitOfWork.CompleteAsync();

            return rowsAffected > 0 ? attempt : null;
        }

        private ExamToAttemptDto MapExamToAttemptDto(Exam exam, ExamAttempt attempt)
        {
            var examToAttempt = _mapper.Map<ExamToAttemptDto>(exam);
            examToAttempt.StartedAt = attempt.StartedAt;
            examToAttempt.ID = attempt.Id;
            return examToAttempt;
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

        private void UpdateAttemptWithResults(ExamAttempt attempt)
        {
            attempt.Percentage = attempt.MaxScore > 0
                ? ((double)attempt.Score / attempt.MaxScore) * 100
                : 0;
            attempt.IsSucceed = attempt.Percentage >= attempt.Exam.PassingPercentage;
            attempt.IsCompleted = true;
            attempt.SubmittedAt = DateTime.UtcNow;
        }

        #endregion

        #region Database Operations

        private async Task<bool> SaveExamSubmissionAsync(ExamAttempt attempt, List<StudentAnswer> studentAnswers)
        {
            await _unitOfWork.Repository<ExamAttempt>().UpdatePartialAsync(attempt);
            await SaveStudentAnswersAsync(studentAnswers);

            var rowsAffected = await _unitOfWork.CompleteAsync();
            return rowsAffected > 0;
        }

        private async Task SaveStudentAnswersAsync(List<StudentAnswer> studentAnswers)
        {
            foreach (var studentAnswer in studentAnswers)
            {
                await _unitOfWork.Repository<StudentAnswer>().AddAsync(studentAnswer);
            }
        }

        #endregion

        #region Query Helper Methods

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

        #endregion
    }
}*/