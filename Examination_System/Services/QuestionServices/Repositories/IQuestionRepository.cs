using Examination_System.DTOs.Question;
using Examination_System.Models;

namespace Examination_System.Services.QuestionServices.Repositories
{
    public interface IQuestionRepository
    {
        Task<Question?> GetByIdAsync(Guid questionId);
        Task<Question?> GetByIdAndInstructorAsync(Guid questionId, Guid instructorId);
        Task<List<QuestionToReturnDto>> GetByInstructorAsync(Guid instructorId);
        Task<List<QuestionToReturnDto>> GetByInstructorAndCourseAsync(Guid instructorId, Guid courseId);
        Task<int> GetExamUsageCountAsync(Guid questionId);
        Task AddAsync(Question question);
        Task UpdateAsync(Question question);
        Task<bool> DeleteAsync(Guid questionId);
        Task DeleteChoicesAsync(IEnumerable<Guid> choiceIds);
        Task<int> SaveChangesAsync();
    }
}