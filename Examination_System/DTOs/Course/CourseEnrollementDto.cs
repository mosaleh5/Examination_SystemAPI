namespace Examination_System.DTOs.Course
{
    public class CourseEnrollementDto
    {
        public Guid CourseId { get; set; }
        public Guid InstructorId { get; set; }
        public Guid StudentId { get; set; }
    }
}
