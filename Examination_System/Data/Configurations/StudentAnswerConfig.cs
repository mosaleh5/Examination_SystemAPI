using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Examination_System.Data.Configurations
{
    public class StudentAnswerConfig : IEntityTypeConfiguration<StudentAnswer>
    {
        public void Configure(EntityTypeBuilder<StudentAnswer> builder)
        {
            builder.HasKey(sa => sa.Id);

            // Create a composite unique index on AttemptId and QuestionId 
            // to ensure one answer per question per attempt
            builder.HasIndex(sa => new { sa.AttemptId, sa.QuestionId })
                .IsUnique()
                .HasDatabaseName("IX_StudentAnswers_AttemptId_QuestionId");

            // Relationships
            builder.HasOne(sa => sa.ExamAttempt)
                .WithMany(ea => ea.StudentAnswers)
                .HasForeignKey(sa => sa.AttemptId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sa => sa.Question)
                .WithMany()
                .HasForeignKey(sa => sa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sa => sa.SelectedChoice)
                .WithMany()
                .HasForeignKey(sa => sa.SelectedChoiceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
