using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Examination_System.Data.Configurations
{
    public class CourseConfig : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {

            builder.HasMany(ce => ce.CourseEnrollments)
                .WithOne(c => c.Course)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
