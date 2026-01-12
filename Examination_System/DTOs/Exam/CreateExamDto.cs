using System.ComponentModel.DataAnnotations;
using Examination_System.Models.Enums;
using Examination_System.Validation;

namespace Examination_System.DTOs.Exam
{
    public class CreateExamDto
    {
        [Required(ErrorMessage = "Exam title is required")]
        [MaxLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course ID is required")]
        public Guid CourseId { get; set; }  // Changed from int

        [Required(ErrorMessage = "Instructor ID is required")]
        public Guid InstructorId { get; set; }  // Changed from string

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        public int DurationInMinutes { get; set; }

        [Required(ErrorMessage = "Full mark is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Full marks must be greater than 0.")]
        public int Fullmark { get; set; }

        [Required(ErrorMessage = "Exam type is required")]
        public ExamType ExamType { get; set; } // 0 = Quiz, 1 = Final

        [Required(ErrorMessage = "Passing score is required")]
        public int PassingPercentage { get; set; }

        [Required(ErrorMessage = "Questions count is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Questions count must be at least 1.")]
        public int QuestionsCount { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}