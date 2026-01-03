using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.ViewModels.Question;

namespace Examination_System.ViewModels.Exam
{
    public class ExamResponseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int DurationInMinutes { get; set; }
        public int Fullmark { get; set; }
        public string ExamType { get; set; } // "Quiz" or "Final"
        public int PassingScore { get; set; }
        public int QuestionsCount { get; set; }

        public string CourseName { get; set; }
        public string InstructorId { get; set; }
        public string InstructorName { get; set; }
        public bool IsAutomatic { get; set; }
        public bool isActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
      
    }
}