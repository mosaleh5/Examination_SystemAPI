using System.ComponentModel.DataAnnotations;
using static Examination_System.Validation.ExamValidation;

namespace Examination_System.DTOs.Exam
{
    public class CreateExamDto
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
        public int ExamType { get; set; } // 0 = Quiz, 1 = Final

        [Required(ErrorMessage = "Passing score is required")]
       
        public int PassingScore { get; set; }

        [Required(ErrorMessage = "Questions count is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Questions count must be at least 1.")]
        public int QuestionsCount { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid course ID")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Instructor ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid instructor ID")]
        public string InstructorId { get; set; }
        public DateTime CreatedAt { get; internal set; }

        [Required(ErrorMessage = "Automatic setting is required")]
        public bool IsAutomatic { get; set; } = false;
    }
}