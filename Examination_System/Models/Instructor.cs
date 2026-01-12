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
        public User User { get; set; }
        
        // Relations
        public ICollection<Course> Courses { get; set; }
        public ICollection<Exam> Exams { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
