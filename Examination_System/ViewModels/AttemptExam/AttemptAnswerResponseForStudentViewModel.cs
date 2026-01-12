using Examination_System.DTOs.Question;
using Examination_System.ViewModels.Question;

namespace Examination_System.ViewModels.AttemptExam
{
    public class AttemptAnswerResponseForStudentViewModel
    {
        public QuestionToReturnViewModel Question { get; set; }

        public string ChoiceAnswer { get; set; }

        public bool IsCorrect { get; set; }
        
    }
}
