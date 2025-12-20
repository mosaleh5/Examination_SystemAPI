using Examination_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Data.Configurations
{
    public class InstructorConfig : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Instructor> builder)
        {
            builder.HasMany(c => c.Courses)
                .WithOne(c => c.Instructor)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
