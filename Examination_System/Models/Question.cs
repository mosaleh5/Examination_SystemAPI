using Examination_System.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Examination_System.Models
{
    public partial class Question : BaseModelGuid
    {

        [Required]
        [MaxLength(500)]
        public string Title { get; set; }
        [Required]
        public int Mark { get; set; }
        [Required]
        [ValidateEnum]
        public QuestionLevel Level { get; set; }

        [ValidateOneChoiceIsCorrect]
        public ICollection<Choice> Choices { get; set; } = new List<Choice>();
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

        public Instructor Instructor { get; set; }
        [ForeignKey(nameof(Instructor))]
        public Guid InstructorId { get; set; }

        public Course Course { get; set; }
        [ForeignKey(nameof(Course))]
        public Guid? CourseId { get; set; }
    }
}
