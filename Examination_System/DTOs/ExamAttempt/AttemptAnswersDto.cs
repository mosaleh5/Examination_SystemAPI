using Examination_System.DTOs.Question;
using Examination_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.DTOs.ExamAttempt
{
    public class AttemptAnswersDto
    {
       public QuestionToReturnForStudentDto Question { get; set; }
       
       public string SelectedChoiceText { get; set; }

        public bool IsCorrect { get; set; }
    }
}
