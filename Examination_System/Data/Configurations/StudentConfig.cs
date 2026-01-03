using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Examination_System.Data.Configurations
{
    public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasMany(s => s.ExamAssignments)
                .WithOne(eg => eg.Student)
                .HasForeignKey(eg => eg.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.ExamAttempt)
             .WithOne(eg => eg.Student)
             .HasForeignKey(eg => eg.StudentId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.CourseEnrollments)
                .WithOne(ce => ce.Student)
                .HasForeignKey(ce => ce.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => s.Id)
                    .IsUnique()
                    .HasFilter("[IsDeleted] = 0");
        }
    }
}
