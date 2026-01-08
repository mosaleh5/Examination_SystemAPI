using Examination_System.Common;
using Examination_System.DTOs.Question;

namespace Examination_System.Services.QuestionServices
{
    public interface IQuestionServices
    {

        Task<Result<QuestionToReturnDto>> CreateQuestionAsync(CreateQuestionDto createQuestionDto);
        Task<Result<IEnumerable<QuestionToReturnDto>>> GetQuestionsByInstructorAsync(string? instructorId);
        Task<Result<IEnumerable<QuestionToReturnDto>>> GetQuestionsByInstructorAndCourseAsync(string? instructorId,int? CourseId);
        Task<Result<QuestionToReturnDto>> UpdateQuestionAsync(UpdateQuestionDto updateQuestionDto);
        Task<Result> DeleteQuestionAsync(int questionId, string ? instructorId);
        Task<Result<QuestionToReturnDto>> GetQuestionByIdAsync(int questionId, string instructorId);
    }
}
