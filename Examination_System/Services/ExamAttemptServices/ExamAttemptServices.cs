using AutoMapper;
using Examination_System.Common;
using Examination_System.Common.Constants;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Services.ExamAttemptServices.Repositories;
using Examination_System.Services.ExamAttemptServices.Validators;

namespace Examination_System.Services.ExamAttemptServices
{
    /// <summary>
    /// Service for managing exam attempts including starting, submitting, and retrieving attempts
    /// </summary>
    public class ExamAttemptServices : GenericServices<ExamAttempt>, IExamAttemptServices
    {
        private readonly IExamAttemptRepository _attemptRepository;
        private readonly IExamAvailabilityValidator _availabilityValidator;
        private readonly IExamSubmissionValidator _submissionValidator;
        private readonly IExamEvaluator _examEvaluator;

        public ExamAttemptServices(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IExamAttemptRepository attemptRepository,
            IExamAvailabilityValidator availabilityValidator,
            IExamSubmissionValidator submissionValidator,
            IExamEvaluator examEvaluator) : base(unitOfWork, mapper)
        {
            _attemptRepository = attemptRepository;
            _availabilityValidator = availabilityValidator;
            _submissionValidator = submissionValidator;
            _examEvaluator = examEvaluator;
        }

        /// <summary>
        /// Retrieves an exam attempt by its ID
        /// </summary>
        public async Task<Result<ExamAttemptDto>> GetAttemptByIdAsync(Guid attemptId)
        {
            var attempt = await _attemptRepository.GetAttemptWithDetailsAsync(attemptId);

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
            if (!await _attemptRepository.IsStudentEnrolledInExamAsync(examId, studentId))
            {
                return Result<ExamToAttemptDto>.Failure(
                    ErrorCode.Forbidden,
                    string.Format(ErrorMessages.StudentNotEnrolled, studentId, examId));
            }

          
            var exam = await _attemptRepository.GetExamByIdAsync(examId);
            if (exam == null)
            {
                return Result<ExamToAttemptDto>.Failure(
                    ErrorCode.NotFound,
                    string.Format(ErrorMessages.ExamNotFound, examId));
            }

            var validationResult = await _availabilityValidator.ValidateAsync(exam, studentId);
            if (!validationResult.IsSuccess)
            {
                return Result<ExamToAttemptDto>.Failure(validationResult.Error, validationResult.ErrorMessage);
            }

            var attempt = await CreateAttemptAsync(exam.Id, studentId);
            if (attempt == null)
            {
                return Result<ExamToAttemptDto>.Failure(
                    ErrorCode.DatabaseError,
                    ErrorMessages.ExamStartFailed);
            }

            var examToAttempt = MapToExamToAttemptDto(exam);
            examToAttempt.StartedAt = attempt.StartedAt;
            examToAttempt.ID = attempt.Id;
            
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
            var attempt = await _attemptRepository.GetAttemptWithDetailsAsync(attemptId);
            if (attempt == null)
            {
                return Result<ExamAttemptDto>.Failure(
                    ErrorCode.NotFound,
                    string.Format(ErrorMessages.ExamAttemptNotFound, attemptId));
            }

            var validationResult = _submissionValidator.Validate(attempt, answers);
            if (!validationResult.IsSuccess)
            {
                return Result<ExamAttemptDto>.Failure(validationResult.Error, validationResult.ErrorMessage);
            }

            var questions = await _attemptRepository.GetQuestionsByIdsAsync(
                answers.Select(a => a.QuestionId).ToList());

            var evaluation = await _examEvaluator.EvaluateAsync(attempt, answers, questions);
            
            ApplyEvaluationToAttempt(attempt, evaluation);

            // Save changes
            if (!await SaveSubmissionAsync(attempt, evaluation.StudentAnswers))
            {
                return Result<ExamAttemptDto>.Failure(
                    ErrorCode.DatabaseError,
                    ErrorMessages.ExamSubmitFailed);
            }

            // Return updated attempt
            var submittedAttempt = await _attemptRepository.GetAttemptWithDetailsAsync(attemptId);
            var attemptDto = _mapper.Map<ExamAttemptDto>(submittedAttempt);
            attemptDto.PassingPercentage = attempt.Exam.PassingPercentage;

            return Result<ExamAttemptDto>.Success(attemptDto);
        }

        /// <summary>
        /// Gets all exam attempts for exams created by a specific instructor
        /// </summary>
        public async Task<Result<IEnumerable<ExamAttemptDto>>> GetStudentAttemptsForInstructorAsync(Guid instructorId)
        {
            var instructorExamIds = await _attemptRepository.GetInstructorExamIdsAsync(instructorId);
            var attemptsList = await _attemptRepository.GetAttemptsByExamIdsAsync(instructorExamIds);

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
            var instructorExamIds = await _attemptRepository.GetInstructorExamIdsAsync(instructorId);
            var attemptsList = await _attemptRepository.GetAttemptsByExamIdsAndStudentAsync(instructorExamIds, studentId);

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
            var attemptsList = await _attemptRepository.GetAttemptsByStudentIdAsync(studentId);

            if (!attemptsList.Any())
            {
                return Result<IEnumerable<ExamAttemptDto>>.Failure(
                    ErrorCode.NotFound,
                    string.Format(ErrorMessages.NoAttemptsFound, $"student {studentId}"));
            }

            return Result<IEnumerable<ExamAttemptDto>>.Success(attemptsList);
        }

        #region Private Helper Methods

        private async Task<ExamAttempt?> CreateAttemptAsync(Guid examId, Guid studentId)
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

       private ExamToAttemptDto MapToExamToAttemptDto(Exam exam)
        {
            return new ExamToAttemptDto
            {
                ExamId = exam.Id,
                Title = exam.Title,
                DurationInMinutes = exam.DurationInMinutes,
                QuestionsCount = exam.QuestionsCount,
                Questions = exam.ExamQuestions?
                    .Where(eq => eq.Question != null && !eq.Question.IsDeleted)
                    .Select(eq => _mapper.Map<QuestionToAttemptDto>(eq.Question))
                    .ToList() ?? new List<QuestionToAttemptDto>()
            };
        }

        private void ApplyEvaluationToAttempt(ExamAttempt attempt, ExamEvaluationResult evaluation)
        {
            attempt.Score = evaluation.TotalScore;
            attempt.MaxScore = evaluation.MaxScore;
            attempt.Percentage = evaluation.Percentage;
            attempt.IsSucceed = evaluation.Percentage >= attempt.Exam.PassingPercentage;
            attempt.IsCompleted = true;
            attempt.SubmittedAt = DateTime.UtcNow;
        }

        private async Task<bool> SaveSubmissionAsync(ExamAttempt attempt, List<StudentAnswer> studentAnswers)
        {
            await _unitOfWork.Repository<ExamAttempt>().UpdatePartialAsync(attempt);
            
            foreach (var studentAnswer in studentAnswers)
            {
                await _unitOfWork.Repository<StudentAnswer>().AddAsync(studentAnswer);
            }

            var rowsAffected = await _unitOfWork.CompleteAsync();
            return rowsAffected > 0;
        }

        #endregion
    }
}