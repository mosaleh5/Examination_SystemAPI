using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.Models
{
    // StudentAnswer Entity (Student's answer to a question in an attempt)
    public class StudentAnswer : BaseModel
    {
  

        [ForeignKey("ExamAttempt")]
        public int AttemptId { get; set; }
        public ExamAttempt ExamAttempt { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [ForeignKey("Choice")]
        public int? SelectedChoiceId { get; set; }
        public Choice SelectedChoice { get; set; }

        public bool IsCorrect { get; set; }

      
    }
}
