namespace Examination_System.DTOs.Exam
{
    public class ExamDetailsDto
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int DurationInMinutes { get; set; }
        public int Fullmark { get; set; }
        public int PassingScore { get; set; }
        public string ExamType { get; set; }
        public int QuestionsCount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Course Info
        public int CourseId { get; set; }
        public string CourseName { get; set; }
  
        // Instructor Info
        public int InstructorId { get; set; }
        public string InstructorName { get; set; }
       

        // Statistics
        public int AssignedStudentsCount { get; set; }
        public int CompletedAttemptsCount { get; set; }
        public int TotalQuestionsCount { get; set; }

        // Related Questions
        public List<ExamQuestionDto> Questions { get; set; } = new List<ExamQuestionDto>();

        // Assigned Students
        public List<AssignedStudentDto> AssignedStudents { get; set; } = new List<AssignedStudentDto>();
    }

    public class ExamQuestionDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public int Points { get; set; }
    }

    public class AssignedStudentDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Major { get; set; }
        public bool HasAttempted { get; set; }
        public int? Score { get; set; }
    }
}