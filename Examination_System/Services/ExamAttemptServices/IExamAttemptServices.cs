using Examination_System.Common;
using Examination_System.DTOs.ExamAttempt;

namespace Examination_System.Services.ExamAttemptServices
{
    public interface IExamAttemptServices
    {
        Task<Result<ExamToAttemptDto>> StartExamAsync(Guid examId, Guid userId);
        Task<Result<ExamAttemptDto>> SubmitExamAsync(Guid attemptId, List<SubmitAnswerDto> answers);
        Task<Result<ExamAttemptDto>> GetAttemptByIdAsync(Guid attemptId);
        Task<Result<IEnumerable<ExamAttemptDto>>> GetStudentAttemptsForInstructorAsync(Guid instructorId);
        Task<Result<IEnumerable<ExamAttemptDto>>> GetStudentAttemptsAsync(Guid instructorId, Guid studentId);
        Task<Result<IEnumerable<ExamAttemptDto>>> GetStudentAttemptsForStudentAsync(Guid userId);
    }
}