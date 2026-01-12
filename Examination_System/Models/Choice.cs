using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.Models
{
    public class Choice : BaseModelGuid
    {

        [Required]
        public string Text { get; set; } = null!;
        [Required]

        public bool IsCorrect { get; set; }
        [ForeignKey(nameof(QuestionId))]
        public Question Question { get; set; }
        public Guid QuestionId { get; set; }
      

        public ICollection<StudentAnswer>? StudentAnswers { get; set; }
     
    }
}
