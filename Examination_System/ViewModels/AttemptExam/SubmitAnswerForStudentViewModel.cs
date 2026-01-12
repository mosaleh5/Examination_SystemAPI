using Microsoft.Identity.Client;
using System;

namespace Examination_System.ViewModels.AttemptExam
{
    public class SubmitAnswerForStudentViewModel
    {
        public Guid QuestionId { get; set; }  
        public Guid ChoiceId { get; set; }  
    }
}
