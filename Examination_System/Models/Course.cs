using System.ComponentModel.DataAnnotations;

namespace Examination_System.Models
{
    public class Course : BaseModel
    {
       
        [Required]
        public string Hours { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public Instructor? Instructor { get; set; }
        public int InstructorId { get; set; }

        public ICollection<Exam>? Exams { get; set; }
        public ICollection<CourseEnrollment>? CourseEnrollments { get; set; }
        
    }
}
