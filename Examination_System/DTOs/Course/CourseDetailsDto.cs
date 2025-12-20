namespace Examination_System.DTOs.Course
{
    public class CourseDetailsDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Hours { get; set; }
        public DateTime CreatedAt { get; set; }

        // Instructor information
        public int InstructorId { get; set; }
        public string InstructorName { get; set; }
        public string InstructorEmail { get; set; }

        // Statistics
        public int EnrolledStudentsCount { get; set; }
        public int ExamsCount { get; set; }

        // Related data
        public List<CourseExamDto> Exams { get; set; }
        public List<EnrolledStudentDto> EnrolledStudents { get; set; }
    }

    public class CourseExamDto
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int DurationMinutes { get; set; }
        public string ExamType { get; set; }
    }

    public class EnrolledStudentDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Major { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
