using System.ComponentModel.DataAnnotations;

namespace Examination_System.ViewModels.AttemptExam
{
    public class ExamAttemptResponseForStudentViewModel
    {
 
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedAt { get; set; }

        [Range(0, int.MaxValue)]
        public int? Score { get; set; }

        [Range(0, int.MaxValue)]
        public int? MaxScore { get; set; }

        public bool IsCompleted { get; set; } = false;
        public double? Percentage { get; set; }
        public bool IsSucceed { get; set; }

        public List<AttemptAnswerResponseForStudentViewModel> Answers { get; set; }
    }
}
