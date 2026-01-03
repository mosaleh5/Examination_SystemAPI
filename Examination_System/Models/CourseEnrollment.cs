using System.ComponentModel.DataAnnotations;

namespace Examination_System.Models
{
    public class CourseEnrollment : BaseModel
    {

   
        [Required]

        public string StudentId { get; set; }
        [Required]
        public Student Student { get; set; }
        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    }
}
