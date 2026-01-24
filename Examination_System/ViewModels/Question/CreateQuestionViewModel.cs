using Examination_System.Models;
using Examination_System.Attributes;
using Examination_System.ViewModels.Choice;
using System.ComponentModel.DataAnnotations;
using static Examination_System.Models.Question;

namespace Examination_System.ViewModels.Question
{
    public class CreateQuestionViewModel
    {
        public string Title { get; set; } = string.Empty;
        public QuestionLevel Level { get; set; } 
        public int Mark { get; set; }
        public Guid CourseId { get; set; } 
        public ICollection<CreateChoiceViewModel> Choices { get; set; } = new List<CreateChoiceViewModel>();
    }
    
    public class CreateChoiceViewModel
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}