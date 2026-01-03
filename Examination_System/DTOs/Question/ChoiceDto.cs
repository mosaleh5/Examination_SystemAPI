using System.ComponentModel.DataAnnotations;

namespace Examination_System.DTOs.Question
{
    public class ChoiceDto
    {
        int Id;
        [Required]
        public string Text { get; set; } 
        [Required]
        public bool IsCorrect { get; set; }
        [Required]
        public int? QuestionId { get; set; }
    }
}
