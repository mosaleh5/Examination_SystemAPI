namespace Examination_System.DTOs.ExamAttempt
{
    public class ChoiceToReturnForStudentDto
    {
        public int Id { get; set; }
        public string Text { get; set; }     
        public int? QuestionId { get; set; }
    }
}
