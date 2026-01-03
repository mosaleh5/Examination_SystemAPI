using Examination_System.DTOs.ExamAttempt;
using Examination_System.Validation;
using System.ComponentModel.DataAnnotations;
using static Examination_System.Models.Question;

namespace Examination_System.DTOs.Question
{
    public class QuestionToReturnForStudentDto
    {
        public int Id { get; set; }    
        public string Title { get; set; }
        public int Mark { get; set; }
        public ICollection<ChoiceToReturnForStudentDto> Choices { get; set; }
    }
}
