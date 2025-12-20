using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Examination_System.Data.Configurations
{
    public class QuestionConfig : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasMany(c => c.Choices)
                .WithOne(c => c.Question)
                .HasForeignKey(sc => sc.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(eq => eq.ExamQuestions)
                .WithOne(eq => eq.Question)
                .HasForeignKey(eq => eq.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(q => q.Instructor)
                .WithMany(i => i.Questions)
                .HasForeignKey(q => q.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
