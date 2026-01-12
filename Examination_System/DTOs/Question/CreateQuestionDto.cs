using Examination_System.Models;
using Examination_System.Validation;
using System.ComponentModel.DataAnnotations;
using static Examination_System.Models.Question;

namespace Examination_System.DTOs.Question
{
    public class CreateQuestionDto
    {
        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int mark { get; set; }

        [Required]
        public QuestionLevel Level { get; set; }

        public Guid CourseId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "At least two choices are required")]
        [ValidateOneChoiceIsCorrect(ErrorMessage = "At least one choice must be marked as correct")]
       
        public ICollection<ChoiceDto> Choices { get; set; } = new List<ChoiceDto>();

        [Required]
        public Guid InstructorId { get; set; } 
    }
}
