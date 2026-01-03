using Examination_System.DTOs.ExamAttempt;

namespace Examination_System.Services.ExamAttemptServices
{
    public interface IExamAttemptServices
    {
        Task<ExamToAttemptDto> StartExamAsync(int examId, string userId);
        Task<ExamAttemptDto> SubmitExamAsync(int attemptId, List<SubmitAnswerDto> answers);
        Task<ExamAttemptDto> GetAttemptByIdAsync(int attemptId);
        Task<IEnumerable<ExamAttemptDto>> GetStudentAttemptsForInstructorAsync(string instructorId);
        Task<IEnumerable<ExamAttemptDto>> GetStudentAttemptsAsync(string instructorId, string studentId);
        Task<IEnumerable<ExamAttemptDto>> GetStudentAttemptsForStudentAsync(string userId);
    }
}