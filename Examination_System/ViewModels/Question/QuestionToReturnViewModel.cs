using Examination_System.DTOs.Question;
using Examination_System.Models;
using Examination_System.Validation;
using Examination_System.ViewModels.Choice;
using System.ComponentModel.DataAnnotations;
using static Examination_System.Models.Question;

namespace Examination_System.ViewModels.Question
{
    public class QuestionToReturnViewModel
    {
        public Guid Id { get; set; }  // Changed from int
        public string Title { get; set; }
        public QuestionLevel Level { get; set; }
        public int Mark { get; set; }
        public Guid CourseId { get; set; }  // Changed from int
        public ICollection<ChoiceToReturnViewModel> Choices { get; set; } = new List<ChoiceToReturnViewModel>();




       
    }

   
}
