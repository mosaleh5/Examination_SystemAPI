using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.Models
{
    public class Instructor : BaseModelString
    {
        [Key]
        [ForeignKey(nameof(User))]
        public new string Id { get; set; }  // Override to be PK and FK
        
        public User User { get; set; }
        
        public string Department { get; set; }
        public string Specialization { get; set; }
        
        public ICollection<Course> Courses { get; set; }
        public ICollection<Exam> Exams { get; set; }
        public ICollection<Question> Questions { get; set; }

    }
}
