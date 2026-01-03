
using Examination_System.DTOs.Question;

namespace Examination_System.Services.QuestionServices
{
    public interface IQuestionServices
    {

        Task<QuestionToReturnDto> CreateQuestionAsync(CreateQuestionDto createQuestionDto);
        Task<IEnumerable<QuestionToReturnDto>> GetQuestionsByInstructorAsync(string? instructorId);
        Task<IEnumerable<QuestionToReturnDto>> GetQuestionsByInstructorAndCourseAsync(string? instructorId,int? CourseId);

    }
}
