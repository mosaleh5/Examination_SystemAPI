using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.Models
{
    public class Student : BaseModel
    {
      
        public User User { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public string Major { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public ICollection<ExamAssignment> Exams { get; set; }
        public ICollection<CourseEnrollment> Courses { get; set; }

        public int ExamId { get; set; }

       
    }
}
