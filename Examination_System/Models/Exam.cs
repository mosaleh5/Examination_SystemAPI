using Examination_System.Models.Enums;
using Examination_System.Validation;
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
        [ValidateEnumAttribute]
        public ExamType ExamType { get; set; } // Quiz / Final

        [Required]
        //  [PassingScoreAttribute(nameof(Fullmark))]
        [Range(0, 100, ErrorMessage = "Passing percentage must be between 0 and 100.")]
        public int PassingPercentage { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Questions count must be at least 1.")]
        public int QuestionsCount { get; set; }
        [Required]
        public bool IsAutomatic  { get; set; }
        [Required]
        public bool IsActive { get; set; } = false;

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [ForeignKey(nameof(Instructor))]
        public string InstructorId { get; set; }
        public Instructor? Instructor { get; set; }

        public ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();
        public ICollection<ExamAssignment> StudentExams { get; set; } = new List<ExamAssignment>();
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    }

    
}
