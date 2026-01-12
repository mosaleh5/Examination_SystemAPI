namespace Examination_System.DTOs.Exam
{
    public class AddQuestionsToExamDto
    {
        public Guid ExamId { get; set; }  
        public List<Guid> QuestionIds { get; set; } = new();  
        public Guid InstructorId { get; set; } 
    }
}