namespace Examination_System.DTOs.ExamAttempt
{
    public class StudentAnswerToReturnDto
    {

        public Guid QuestionId { get; set; }
        public Guid? SelectedChoiceId { get; set; }
        public bool IsCorrect { get; set; }
    }
}
