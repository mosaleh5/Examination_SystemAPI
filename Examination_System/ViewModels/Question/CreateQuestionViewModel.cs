using Examination_System.Models;
using Examination_System.Validation;
using Examination_System.ViewModels.Choice;
using System.ComponentModel.DataAnnotations;
using static Examination_System.Models.Question;

namespace Examination_System.ViewModels.Question
{
    public class CreateQuestionViewModel
    {
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }
        
        [Required]
        [Range(1, 100)]
        public int Mark { get; set; }
        
        [Required]
        public QuestionLevel Level { get; set; }
        
        [Required]
        public int CourseId { get; set; } // User selects from dropdown
        
        [Required]
        [MinLength(2, ErrorMessage = "At least two choices are required")]
        [ValidateOneChoiceIsCorrectAttribute(ErrorMessage = "only one choice must be marked as correct")]
        public ICollection<ChoiceViewModel> Choices { get; set; } = new List<ChoiceViewModel>();
    }
}