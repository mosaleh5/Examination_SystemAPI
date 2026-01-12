namespace Examination_System.DTOs.Exam
{
    public class RemoveQuestionFromExamDto
    {
        public Guid ExamId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid InstructorId { get; set; } 
    }
}