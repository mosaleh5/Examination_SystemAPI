using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.Models
{
    public class Instructor : BaseModelGuid
    {
        public Instructor()
        {
            Id = Guid.CreateVersion7();
        }

        public string Department { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        
        // Navigation property
        public User User { get; set; } = null!;
        
        // Relations
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
