using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Examination_System.Data.Configurations
{
    public class CourseEnrollmentConfig : IEntityTypeConfiguration<CourseEnrollment>
    {
        public void Configure(EntityTypeBuilder<CourseEnrollment> builder)
        {
            builder.HasOne(ce => ce.Student)
                .WithMany(s => s.Courses)
                .HasForeignKey(ce => ce.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ce => ce.Course)
                .WithMany(c => c.CourseEnrollments)
                .HasForeignKey(ce => ce.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
