using Examination_System.ViewModels.Question;

namespace Examination_System.ViewModels.AttemptExam
{
    public class ExamToAttemptDetailedResponseForStudentViewModel
    {
        public Guid ID { get; set; }  
        public Guid ExamId { get; set; }  
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
        public DateTime StartedAt { get; set; }
        public ICollection<QuestionForAttemptViewModel> Questions { get; set; } = new List<QuestionForAttemptViewModel>();
    }
    
    public class QuestionForAttemptViewModel
    {
        public Guid Id { get; set; }  
        public string Text { get; set; } = string.Empty;
        public int Mark { get; set; }
        public ICollection<ChoiceForAttemptViewModel> Choices { get; set; } = new List<ChoiceForAttemptViewModel>();
    }
    
    public class ChoiceForAttemptViewModel
    {
        public Guid Id { get; set; } 
        public string Text { get; set; } = string.Empty;
    }
}
