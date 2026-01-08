using Examination_System.Validation;
using Examination_System.ViewModels;
using System.ComponentModel.DataAnnotations;
using static Examination_System.Models.Question;

namespace Examination_System.DTOs.Question
{
    public class QuestionToReturnDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }

        [Required]
        [Range(1, 100)]
        public int Mark { get; set; }

        [Required]
        public QuestionLevel Level { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "At least two choices are required")]
        [ValidateOneChoiceIsCorrect(ErrorMessage = "At least one choice must be marked as correct")]
        public ICollection<ChoiceToReturnForInstructorDto> Choices { get; set; } 
    }
}