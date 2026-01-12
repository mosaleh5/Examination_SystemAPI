namespace Examination_System.DTOs.Student
{
    public class ExamAssignmentDto
    {
        public Guid Id { get; set; } 
        public Guid ExamId { get; set; } 
        public string ExamTitle { get; set; } = string.Empty;
        public string ExamDescription { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsActive { get; set; }
        public int DurationInMinutes { get; set; }
        public int QuestionsCount { get; set; }
    }
}