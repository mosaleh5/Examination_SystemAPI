namespace Examination_System.DTOs.ExamAttempt
{
    public class SubmitAnswerDto
    {
        public Guid AttemptId { get; set; }
        public Guid QuestionId { get; set; } 
        public Guid ChoiceId { get; set; } 
    }
}