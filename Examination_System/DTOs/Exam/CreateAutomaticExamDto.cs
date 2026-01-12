using System.ComponentModel.DataAnnotations;
using Examination_System.Validation;
using Examination_System.Models.Enums;

namespace Examination_System.DTOs.Exam
{
    public class CreateAutomaticExamDto
    {
        [Required(ErrorMessage = "Exam title is required")]
        [MaxLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Exam date is required")]
        [FutureDate(ErrorMessage = "Exam date must be in the future.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        public int DurationInMinutes { get; set; }

        [Required(ErrorMessage = "Full mark is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Full marks must be greater than 0.")]
        public int Fullmark { get; set; }

        [Required(ErrorMessage = "Exam type is required")]
        public ExamType ExamType { get; set; } // 0 = Quiz, 1 = Final

        [Required(ErrorMessage = "Passing persantage is required")]

        public int PassingPercentage { get; set; }

        [Required(ErrorMessage = "Questions count is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Questions count must be at least 1.")]
        public int QuestionsCount { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "Instructor ID is required")]
        public Guid InstructorId { get; set; }
        
        public DateTime CreatedAt { get; internal set; }

        [Required(ErrorMessage = "Automatic setting is required")]
        public bool IsAutomatic { get; set; } = true;
    }
}