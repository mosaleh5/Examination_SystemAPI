using Examination_System.Models;
using Examination_System.Validation;
using System.ComponentModel.DataAnnotations;
using static Examination_System.Models.Question;

namespace Examination_System.DTOs.Question
{
    public class UpdateQuestionDto
    {
        public Guid Id { get; set; } 
        public string Text { get; set; } = string.Empty;
        public QuestionLevel Level { get; set; }
        public int Mark { get; set; }
        public Guid CourseId { get; set; }  
        public Guid InstructorId { get; set; }  
        public ICollection<UpdateChoiceDto> Choices { get; set; } = new List<UpdateChoiceDto>();
       
    }
}
