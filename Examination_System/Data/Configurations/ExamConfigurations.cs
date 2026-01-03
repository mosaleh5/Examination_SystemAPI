using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Examination_System.Data.Configurations
{
    public class ExamConfigurations : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.Property(e => e.DurationInMinutes)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(e => e.Fullmark)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(e => e.QuestionsCount)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(e => e.Title)
                .HasMaxLength(150)
                .IsRequired();

            builder.HasMany(c => c.ExamQuestions)
                .WithOne()
                .HasForeignKey(sc => sc.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.StudentExams)
                .WithOne(e => e.Exam)
                .HasForeignKey(e => e.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Instructor)
                .WithMany(i => i.Exams)
                .HasForeignKey(e => e.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Course)
                .WithMany(c => c.Exams)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

