using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.Models
{
    public class Choice : BaseModel
    {

        [Required]
        public string Text { get; set; } = null!;
        [Required]

        public bool IsCorrect { get; set; }
        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public ICollection<StudentAnswer> StudentAnswers { get; set; }
        public int StudentAnswerId
        {
            get; set;
        }
    }
}
