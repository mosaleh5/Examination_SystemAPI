using Examination_System.DTOs.Exam;
using Examination_System.DTOs.ExamAttempt;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Services.ExamServices
{
    public interface IExamServices
    {
        Task CreateExamAsync(CreateExamDto createExamDto);
        Task<ActionResult<ExamToReturnDto>> CreateAutomaticExam(CreateAutomaticExamDto createExamDto);
        Task<ActionResult<ExamToReturnDto>> CreateExam(CreateExamDto createExamDto);
        Task<IEnumerable<ExamToReturnDto>> GetAllExamsForInstructor(string? instructorId);
        Task<bool> EnrollStudentToExamAsync(int examId, string studentId);
  
        Task<bool> IsInstructorOfExamAsync(int examId, string instructorId);
        Task<bool> ActivateExamAsync(int examId, string instructorId);
        Task AddQuestionsToExamAsync(int examId, List<int> questionIds);
        Task ReplaceExamQuestionsAsync(int examId, List<int> questionIds);
        Task RemoveQuestionFromExamAsync(int examId, int questionId);
    }
}