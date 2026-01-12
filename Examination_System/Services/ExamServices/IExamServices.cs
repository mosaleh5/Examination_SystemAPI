using Examination_System.Common;
using Examination_System.DTOs.Exam;

namespace Examination_System.Services.ExamServices
{
    public interface IExamServices
    {
        Task<Result<ExamToReturnDto>> CreateAutomaticExam(CreateAutomaticExamDto createExamDto);
        Task<Result<ExamToReturnDto>> CreateExam(CreateExamDto createExamDto);
        Task<Result<IEnumerable<ExamToReturnDto>>> GetAllExamsForInstructor(Guid? instructorId);
        Task<Result> EnrollStudentToExamAsync(AssignStudentToExamDto assignedStudentDto);
        Task<Result> ActivateExamAsync(ActivateExamDto activateExamDto);
        Task<Result> AddQuestionsToExamAsync(AddQuestionsToExamDto addQuestionsDto);
        Task<Result> ReplaceExamQuestionsAsync(ReplaceExamQuestionsDto replaceQuestionsDto);
        Task<Result> RemoveQuestionFromExamAsync(RemoveQuestionFromExamDto removeQuestionDto);
        Task<Result<ExamToReturnDto>> GetExamsForInstructorById(GetExamByIdDto getExamByIdDto);
    }
}