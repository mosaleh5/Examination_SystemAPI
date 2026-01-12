using Examination_System.DTOs.Question;

namespace Examination_System.DTOs.Exam
{
    public class ExamDtoToReturn
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int DurationInMinutes { get; set; }
        public int Fullmark { get; set; }
        public int PassingPercentage { get; set; }
        public string ExamType { get; set; } // "Quiz" or "Final"
        public int QuestionsCount { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int InstructorId { get; set; }
        public string InstructorName { get; set; }
        public DateTime CreatedAt { get; set; }
     
    }
}