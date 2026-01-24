using Examination_System.Models;
using Examination_System.Attributes;
using Examination_System.ViewModels;
using System.ComponentModel.DataAnnotations;
using static Examination_System.Models.Question;

namespace Examination_System.DTOs.Question
{
    public class QuestionToReturnDto
    {
        public Guid Id { get; set; }  
        public string Title { get; set; } = string.Empty;
        public QuestionLevel Level { get; set; }
        public int Mark { get; set; }
        public Guid CourseId { get; set; }  
        public Guid InstructorId { get; set; } 
        public ICollection<ChoiceDto> Choices { get; set; } = new List<ChoiceDto>();

      
    }
}