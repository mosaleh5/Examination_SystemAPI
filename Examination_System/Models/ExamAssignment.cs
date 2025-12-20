using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.Models
{
    public class ExamAssignment : BaseModel
    {
        [Required]
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [Required]
        [ForeignKey(nameof(Exam))]
        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        [Range(0, 100)]
        public decimal? Score { get; set; }

        public ExamStatus Status { get; set; } = ExamStatus.Pending;
    }

    public enum ExamStatus
    {
        Pending,
        InProgress,
        Submitted,
        Graded
    }
}
