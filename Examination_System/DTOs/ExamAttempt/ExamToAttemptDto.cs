using Examination_System.DTOs.Question;

namespace Examination_System.DTOs.ExamAttempt
{
    public class ExamToAttemptDto
    {
        public int ID { get; set; }
        public string Title { get; set; }
      
        public int DurationInMinutes { get; set; }
        public int Fullmark { get; set; }
        public int PassingPercentage { get; set; }
        public string ExamType { get; set; } // "Quiz" or "Final"
        public int QuestionsCount { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string InstructorId { get; set; }
        public string InstructorName { get; set; }
        public DateTime StartedAt = DateTime.UtcNow;
       


        public ICollection<QuestionToReturnForStudentDto>? ExamQuestions { get; set; }
    }
}
