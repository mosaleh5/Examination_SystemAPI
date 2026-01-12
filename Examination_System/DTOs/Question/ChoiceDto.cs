using System.ComponentModel.DataAnnotations;

namespace Examination_System.DTOs.Question
{
    public class ChoiceDto
    {
        public Guid Id { get; set; }  
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public Guid QuestionId { get; set; }
    }
    
 
    
    public class UpdateChoiceDto
    {
        public Guid Id { get; set; }  // Changed from int
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }

    }
}
