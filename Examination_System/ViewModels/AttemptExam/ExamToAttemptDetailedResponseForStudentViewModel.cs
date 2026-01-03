using Examination_System.ViewModels.Question;

namespace Examination_System.ViewModels.AttemptExam
{
    public class ExamToAttemptDetailedResponseForStudentViewModel : ExamToAttemptResponseForStudentViewModel
    {
        public ICollection<QuestionToReturnViewModel> ExamQuestions { get; set; } = new List<QuestionToReturnViewModel>();

    }
}
