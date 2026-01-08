using Examination_System.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Examination_System.Models
{
    public partial class Question : BaseModel
    {

        [Required]
        [MaxLength(500)]
        public string Title { get; set; }
        [Required]
        public int mark { get; set; }
        [Required]
        [ValidateEnum]
        public QuestionLevel Level { get; set; }

        [ValidateOneChoiceIsCorrect]
        public ICollection<Choice> Choices { get; set; } = new List<Choice>();
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
        [ForeignKey(nameof(InstructorId))]

        public Instructor Instructor { get; set; }
        public string InstructorId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
        public int? CourseId { get; set; }
    }
}
