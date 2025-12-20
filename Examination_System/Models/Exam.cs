using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Examination_System.Validation.ExamValidation;

namespace Examination_System.Models
{
    public class Exam : BaseModel
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [FutureDate(ErrorMessage = "Exam date must be in the future.")]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        public int DurationInMinutes { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Full marks must be greater than 0.")]
        public int Fullmark { get; set; }

        [Required]
        public ExamType ExamType { get; set; } // Quiz / Final

        [Required]
        [PassingScoreAttribute(nameof(Fullmark))]
        public int PassingScore { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Questions count must be at least 1.")]
        public int QuestionsCount { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [ForeignKey(nameof(Instructor))]
        public int InstructorId { get; set; }
        public Instructor? Instructor { get; set; }

        public ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();
        public ICollection<ExamAssignment> StudentExams { get; set; } = new List<ExamAssignment>();
        public ICollection<ExamQuestion> Questions { get; set; } = new List<ExamQuestion>();
    }

    public enum ExamType
    {
        Quiz,
        Final
    }

    
}
