using Examination_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.DTOs.ExamAttempt
{
    public class ExamAttemptDto
    {

        public int ExamId { get; set; }
    

  
        public string StudentId { get; set; }
     

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime? SubmittedAt { get; set; }

        [Range(0, int.MaxValue)]
        public int? Score { get; set; }

        [Range(0, int.MaxValue)]
        public int? MaxScore { get; set; }

        public bool IsCompleted { get; set; } = false;


        public double? Percentage { get; set; }

        public bool IsSucceed { get; set; }
        public ICollection<AttemptAnswersDto> StudentAnswers { get; set; } = new List<AttemptAnswersDto>();
    }
}