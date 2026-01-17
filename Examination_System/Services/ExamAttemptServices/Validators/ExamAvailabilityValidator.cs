using Examination_System.Common;
using Examination_System.Common.Constants;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.ExamAttemptServices.Validators
{
    public interface IExamAvailabilityValidator
    {
        Task<Result<object>> ValidateAsync(Exam exam, Guid studentId);
    }

    public class ExamAvailabilityValidator : IExamAvailabilityValidator
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamAvailabilityValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<object>> ValidateAsync(Exam exam, Guid studentId)
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

        private async Task<bool> IsExamCompletedAsync(Guid examId, Guid studentId)
        {
            return await _unitOfWork.Repository<ExamAttempt>()
                .GetAll()
                .AnyAsync(at => at.ExamId == examId && at.StudentId == studentId && at.IsCompleted);
        }
    }
}