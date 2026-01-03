using Microsoft.Identity.Client;

namespace Examination_System.ViewModels.AttemptExam
{
    public class SubmitAnswerForStudentViewModel
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
    }
}
