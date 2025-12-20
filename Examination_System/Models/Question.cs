using System.ComponentModel.DataAnnotations;

namespace Examination_System.Models
{
    public class Question : BaseModel
    {

        [Required]
        [MaxLength(500)]
        public string Title { get; set; }
        public int mark { get; set; }
        public QuestionLevel Level { get; set; }


        public ICollection<Choice> Choices { get; set; } = new List<Choice>();
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
        public Instructor Instructor { get; set; }
        public int InstructorId { get; set; }
        public StudentAnswer StudentAnswer { get; set; }
        public int StudentAnswerId
        {
            get; set;

        }


        public enum QuestionLevel
        {
            Simple,
            Medium,
            Hard
        }
    }
}
