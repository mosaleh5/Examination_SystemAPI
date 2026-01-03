using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.Models
{
    [Index(nameof(StudentId), nameof(ExamId))]

    public class ExamAssignment : BaseModel
    {
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
        
        public string StudentId { get; set; }  // Changed from StudentId
        public Student Student { get; set; }
        
        public DateTime AssignedDate { get; set; }
        public DateTime? SubmittedDate { get; set; } = null;
        public double? Score { get; set; } = null;
    }
}
