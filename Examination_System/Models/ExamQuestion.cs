namespace Examination_System.Models
{
    public class ExamQuestion : BaseModelGuid
    {
     

        public Guid ExamId { get; set; }
        public Exam Exam { get; set; } = null!;

        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }
}
