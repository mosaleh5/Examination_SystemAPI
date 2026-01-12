using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Models
{
    [Index(nameof(StudentId), nameof(ExamId))]
    public class ExamAttempt : BaseModelGuid
    {
        [ForeignKey("Exam")]
        public Guid ExamId { get; set; }
        [Required]
        public Exam Exam { get; set; } = null!;

        [ForeignKey("Student")]
        public Guid StudentId { get; set; }
        [Required]
        public Student Student { get; set; } = null!;

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime? SubmittedAt { get; set; }

        [Range(0, int.MaxValue)]
        public int? Score { get; set; }

        [Range(0, int.MaxValue)]
        public int? MaxScore { get; set; }

        public bool IsCompleted { get; set; } = false;

       
        public double? Percentage { get; set; }

        public bool IsSucceed { get; set; }
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}
