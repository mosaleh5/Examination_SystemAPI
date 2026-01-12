using Examination_System.DTOs.Question;
using Examination_System.Models;
using Examination_System.Models.Enums;
using System.ComponentModel.DataAnnotations;
//using Examination_System.Enums;


namespace Examination_System.DTOs.Exam
{
    public class ExamToReturnDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
        public Guid InstructorId { get; set; }
        public int DurationInMinutes { get; set; }
        public int QuestionsCount { get; set; }
        public int Fullmark { get; set; }
        public int PassingPercentage { get; set; }
        public ExamType ExamType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<QuestionToReturnDto>? Questions { get; set; }
    }
    
}
