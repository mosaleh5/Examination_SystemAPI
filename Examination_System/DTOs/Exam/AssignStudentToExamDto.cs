namespace Examination_System.DTOs.Exam
{
    public class AssignStudentToExamDto
    {
        public Guid ExamId { get; set; } 
        public Guid StudentId { get; set; } 
        public Guid InstructorId { get; set; }  
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    }
}