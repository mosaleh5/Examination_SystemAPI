namespace Examination_System.DTOs.Exam
{
    public class ExamDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
      
    }
}