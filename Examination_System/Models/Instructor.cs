using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.Models
{
    public class Instructor : BaseModel
    {
        public User User { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }

        public ICollection<Course> Courses { get; set; }
        public ICollection<Exam> Exams { get; set; }
        public ICollection<Question> Questions { get; set; }

    }
}
