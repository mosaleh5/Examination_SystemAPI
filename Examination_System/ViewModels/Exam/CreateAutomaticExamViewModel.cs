using Examination_System.Models.Enums;
using Examination_System.Validation;
using System.ComponentModel.DataAnnotations;

namespace Examination_System.ViewModels.Exam
{
    public class CreateAutomaticExamViewModel
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
        [ValidateEnum]
        public ExamType ExamType { get; set; }



        [Required(ErrorMessage = "Passing score is required")]
        [Range(0, 100, ErrorMessage = "Passing percentage must be between 0 and 100.")]
        public int PassingPercentage { get; set; }

        [Required(ErrorMessage = "Questions count is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Questions count must be at least 1.")]
        public int QuestionsCount { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid course ID")]
        public int CourseId { get; set; }
        [Required(ErrorMessage = "Automatic setting is required")]
        public bool IsAutomatic { get; set; }
    }
}