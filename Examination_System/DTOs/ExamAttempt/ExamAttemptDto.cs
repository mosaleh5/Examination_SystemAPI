using Examination_System.DTOs.Question;

namespace Examination_System.DTOs.ExamAttempt
{
    public class ExamAttemptDto
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ExamTitle { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public double Percentage { get; set; }
        public double PassingPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsSucceed { get; set; }
        
        // Fixed: Single StudentAnswers collection
        public ICollection<StudentAnswerDto> StudentAnswers { get; set; } = new List<StudentAnswerDto>();
    }

    // Unified StudentAnswerDto
    public class StudentAnswerDto
    {
        public Guid ExamAttemtId { get; set; }
        public QuestionToReturnDto Question { get; set; }
        public string ChoiceAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}