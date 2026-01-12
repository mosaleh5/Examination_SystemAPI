namespace Examination_System.DTOs.Course
{
    public class CourseDetailsDto
    {
        public Guid Id { get; set; }  
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Hours { get; set; } = string.Empty;
        public Guid InstructorId { get; set; }  
        public string InstructorName { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public int TotalExams { get; set; }
    }
}
