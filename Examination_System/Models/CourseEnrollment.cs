using System.ComponentModel.DataAnnotations;

namespace Examination_System.Models
{
    public class CourseEnrollment : BaseModelGuid
    {

   
        [Required]

        public Guid StudentId { get; set; }
        [Required]
        public Student Student { get; set; } = null!;
        [Required]
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    }
}
