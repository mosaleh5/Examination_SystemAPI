using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Examination_System.Data.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .HasKey(u => u.ID);

            builder
                .HasIndex(u => u.UserName)
                .IsUnique();

            builder
                .HasIndex(u => u.Email)
                .IsUnique();

            
            builder
                .HasOne(u => u.Instructor)
                .WithOne(i => i.User)
                .HasForeignKey<Instructor>(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

         
            builder
                .HasOne(u => u.Student)
                .WithOne(s => s.User)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
