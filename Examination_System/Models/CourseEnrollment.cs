using System.ComponentModel.DataAnnotations;

namespace Examination_System.Models
{
    public class CourseEnrollment : BaseModel
    {

   
        [Required]

        public int StudentId { get; set; }
        [Required]
        public Student Student { get; set; }
        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime EnrollmentAt { get; set; } = DateTime.UtcNow; 

    }
}
