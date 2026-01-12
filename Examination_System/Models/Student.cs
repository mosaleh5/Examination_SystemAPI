using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Examination_System.Models
{
    public class Student : BaseModelGuid
    {
        [Key]
        [ForeignKey(nameof(User))]
        public Guid Id { get; set; }  // Override to be PK and FK
        
        public User User { get; set; }
        
        public string Major { get; set; }
        public DateTime EnrollmentDate { get; set; }
        
        public ICollection<ExamAssignment> ExamAssignments { get; set; }
        public ICollection<CourseEnrollment> CourseEnrollments { get; set; }
        public ICollection<ExamAttempt> ExamAttempt { get; set; }

    }
}
