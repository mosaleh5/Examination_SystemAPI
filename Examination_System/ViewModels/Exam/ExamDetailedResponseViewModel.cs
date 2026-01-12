using Examination_System.DTOs.Exam;
using Examination_System.ViewModels.Question;

namespace Examination_System.ViewModels.Exam
{
    public class ExamDetailedResponseViewModel : ExamResponseViewModel
    {
        public ICollection<QuestionToReturnViewModel> Questions { get; set; } = new List<QuestionToReturnViewModel>();
    }
}