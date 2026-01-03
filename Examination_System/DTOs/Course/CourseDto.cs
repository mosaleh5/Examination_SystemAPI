namespace Examination_System.DTOs.Course
{
    public class CourseDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Hours { get; set; }
        public string? InstructorId { get; set; }
        public string? InstructorName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
