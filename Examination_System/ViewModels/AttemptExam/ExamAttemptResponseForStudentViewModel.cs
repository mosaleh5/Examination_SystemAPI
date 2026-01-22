using Examination_System.DTOs.ExamAttempt;
using System.ComponentModel.DataAnnotations;

namespace Examination_System.ViewModels.AttemptExam
{
    public class ExamAttemptResponseForStudentViewModel
    {
        public Guid Id { get; set; }  
        public Guid ExamId { get; set; }  
        public Guid StudentId { get; set; }  // Changed from string
        public string StudentName { get; set; } = string.Empty;
        public string ExamTitle { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public double Percentage { get; set; }
        public double PassingPercentage { get; set; }

        public bool IsCompleted { get; set; }
        public bool IsSucceed { get; set; }

        public ICollection<AttemptAnswerResponseForStudentViewModel> StudentAnswers { get; set; }
    }
}
