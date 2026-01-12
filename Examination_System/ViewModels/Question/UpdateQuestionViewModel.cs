using Examination_System.Models;
using Examination_System.Validation;
using Examination_System.ViewModels.Choice;
using System.ComponentModel.DataAnnotations;
using static Examination_System.Models.Question;

namespace Examination_System.ViewModels.Question
{
    public class UpdateQuestionViewModel
    {
        public Guid Id { get; set; }  // Changed from int
        public string Text { get; set; } = string.Empty;
        public QuestionLevel Level { get; set; } 
        public int Mark { get; set; }
        public Guid CourseId { get; set; }  // Changed from int
        public ICollection<UpdateChoiceViewModel> Choices { get; set; } = new List<UpdateChoiceViewModel>();
    }
    
    public class UpdateChoiceViewModel
    {
        public Guid Id { get; set; }  // Changed from int
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
