using Examination_System.DTOs.ExamAttempt;
using Examination_System.Models;

namespace Examination_System.Services.ExamAttemptServices.Repositories
{
    public interface IExamAttemptRepository
    {
        Task<Exam?> GetExamByIdAsync(Guid examId);
        Task<ExamAttempt?> GetAttemptWithDetailsAsync(Guid attemptId);
        Task<List<Guid>> GetInstructorExamIdsAsync(Guid instructorId);
        Task<List<ExamAttemptDto>> GetAttemptsByExamIdsAsync(List<Guid> examIds);
        Task<List<ExamAttemptDto>> GetAttemptsByExamIdsAndStudentAsync(List<Guid> examIds, Guid studentId);
        Task<List<ExamAttemptDto>> GetAttemptsByStudentIdAsync(Guid studentId);
        Task<List<Question>> GetQuestionsByIdsAsync(List<Guid> questionIds);
        Task<bool> IsStudentEnrolledInExamAsync(Guid examId, Guid studentId);
    }
}