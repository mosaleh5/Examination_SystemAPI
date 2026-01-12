using Examination_System.DTOs.Question;

namespace Examination_System.DTOs.ExamAttempt
{
    public class ExamToAttemptDto
    {
        public Guid ID { get; set; }  // Attempt ID - Changed from int
        public Guid ExamId { get; set; }  // Changed from int
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
        public int QuestionsCount { get; set; }
        public DateTime StartedAt { get; set; }
        public ICollection<QuestionToAttemptDto> Questions { get; set; } = new List<QuestionToAttemptDto>();
    }
    
    public class QuestionToAttemptDto
    {
        public Guid Id { get; set; }  // Changed from int
        public string Text { get; set; } = string.Empty;
        public int Mark { get; set; }
        public ICollection<ChoiceToAttemptDto> Choices { get; set; } = new List<ChoiceToAttemptDto>();
    }
    
    public class ChoiceToAttemptDto
    {
        public Guid Id { get; set; }  // Changed from int
        public string Text { get; set; } = string.Empty;
    }
}
