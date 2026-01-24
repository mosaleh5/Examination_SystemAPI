using Examination_System.Models.Enums;
using Examination_System.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Examination_System.ViewModels.Exam
{
    public class CreateExamViewModel
    {
        [Required(ErrorMessage = "Exam title is required")]
        [MaxLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

      
        public Guid CourseId { get; set; }  

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        public int DurationInMinutes { get; set; }

        [Required(ErrorMessage = "Questions count is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Questions count must be at least 1.")]
        public int QuestionsCount { get; set; }

        [Required(ErrorMessage = "Passing score is required")]
        [Range(0, 100, ErrorMessage = "Passing percentage must be between 0 and 100.")]
        public int PassingPercentage { get; set; }

        [Required(ErrorMessage = "Exam type is required")]
        [ValidateEnumAttribute]
        public ExamType ExamType { get; set; } // Quiz / Final

        public DateTime Date { get; set; }
    }
}
