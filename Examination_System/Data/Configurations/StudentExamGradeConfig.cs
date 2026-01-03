using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Examination_System.Data.Configurations
{
    public class ExamAssignmentConfig : IEntityTypeConfiguration<ExamAssignment>
    {
        public void Configure(EntityTypeBuilder<ExamAssignment> builder)
        {
          

     


            builder.HasOne(seg => seg.Exam)
                .WithMany(e => e.StudentExams)
                .HasForeignKey(seg => seg.ExamId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
