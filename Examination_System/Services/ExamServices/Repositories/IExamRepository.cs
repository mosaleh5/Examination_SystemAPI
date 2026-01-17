using Examination_System.DTOs.Exam;
using Examination_System.Models;

namespace Examination_System.Services.ExamServices.Repositories
{
    public interface IExamRepository
    {
        Task<Exam> CreateExamAsync(Exam exam);
        Task<ExamToReturnDto?> GetExamByIdAsync(Guid examId);
        Task<IEnumerable<ExamToReturnDto>> GetAllExamsForInstructorAsync(Guid instructorId);
        Task ActivateExamAsync(Guid examId);
        Task UpdateExamFullMarkAsync(Guid examId, int fullMark);
        Task<bool> IsStudentExistsAsync(Guid studentId);
        Task<bool> IsExamExistsAsync(Guid examId);
    }
}