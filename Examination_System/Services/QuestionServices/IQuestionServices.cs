using Examination_System.Common;
using Examination_System.DTOs.Question;

namespace Examination_System.Services.QuestionServices
{
    public interface IQuestionServices
    {

        Task<Result<QuestionToReturnDto>> CreateQuestionAsync(CreateQuestionDto createQuestionDto);
        Task<Result<IEnumerable<QuestionToReturnDto>>> GetQuestionsByInstructorAsync(Guid? instructorId);
        Task<Result<IEnumerable<QuestionToReturnDto>>> GetQuestionsByInstructorAndCourseAsync(Guid? instructorId, Guid? CourseId);
        Task<Result<QuestionToReturnDto>> UpdateQuestionAsync(UpdateQuestionDto updateQuestionDto);
        Task<Result> DeleteQuestionAsync(Guid questionId, Guid? instructorId);
        Task<Result<QuestionToReturnDto>> GetQuestionByIdAsync(Guid questionId, Guid instructorId);
    }
}
