using System.ComponentModel.DataAnnotations;
using Examination_System.Validation;

namespace Examination_System.DTOs.Exam
{
    public class UpdateExamDto
    {
        [Required]
        public int ID { get; set; }

        [MaxLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
        public string? Title { get; set; }

        [FutureDate(ErrorMessage = "Exam date must be in the future.")]
        public DateTime? Date { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        public int? DurationInMinutes { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Full marks must be greater than 0.")]
        public int? Fullmark { get; set; }

        public int? ExamType { get; set; } // 0 = Quiz, 1 = Final

        public int? PassingScore { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Questions count must be at least 1.")]
        public int? QuestionsCount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid course ID")]
        public int? CourseId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid instructor ID")]
        public int? InstructorId { get; set; }
    }
}