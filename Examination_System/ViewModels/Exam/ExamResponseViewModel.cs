using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.ViewModels.Question;

namespace Examination_System.ViewModels.Exam
{
    public class ExamResponseViewModel
    {
        public Guid Id { get; set; }  // Changed from int
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CourseId { get; set; }  // Changed from int
        public string CourseName { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
        public int QuestionsCount { get; set; }
        public int Fullmark { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}