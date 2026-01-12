namespace Examination_System.DTOs.ExamAttempt
{
    public class ChoiceToReturnForStudentDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }     
        public Guid? QuestionId { get; set; }
    }
}
