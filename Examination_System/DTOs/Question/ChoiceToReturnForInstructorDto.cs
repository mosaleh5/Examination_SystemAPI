using System.ComponentModel.DataAnnotations;

namespace Examination_System.DTOs.Question
{
    public class ChoiceToReturnForInstructorDto
    {
       public int Id { get; set; }
        public string Text { get; set; }

        public bool IsCorrect { get; set; }
  
        public int? QuestionId { get; set; }
    }
}