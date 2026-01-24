using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.Models
{
    [Index(nameof(StudentId), nameof(ExamId))]

    public class ExamAssignment : BaseModelGuid
    {
        public Guid ExamId { get; set; }
        public Exam Exam { get; set; } = null!;
        
        public Guid StudentId { get; set; }  // Changed from StudentId
        public Student Student { get; set; } = null!;
        
        public DateTime AssignedDate { get; set; }
        public DateTime? SubmittedDate { get; set; } = null;
        public double? Score { get; set; } = null;
    }
}
