using Examination_System.DTOs.Question;

namespace Examination_System.ViewModels.AttemptExam
{
    public class AttemptAnswerResponseForStudentViewModel
    {
        public QuestionToReturnForStudentDto Question { get; set; }

        public string ChoiceAnswer { get; set; }

        public bool IsCorrect { get; set; }
    }
}
