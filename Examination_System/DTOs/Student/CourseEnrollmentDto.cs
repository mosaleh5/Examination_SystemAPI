namespace Examination_System.DTOs.Student
{
    public class CourseEnrollmentDto
    {
        public Guid Id { get; set; }  
        public Guid CourseId { get; set; } 
        public string CourseName { get; set; } = string.Empty;
        public string CourseDescription { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public Guid InstructorId { get; set; }  
        public string InstructorName { get; set; } = string.Empty;
    }
}