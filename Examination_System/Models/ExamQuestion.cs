namespace Examination_System.Models
{
    public class ExamQuestion : BaseModelGuid
    {
     

        public Guid ExamId { get; set; }
        public Exam Exam { get; set; }

        public Guid QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
