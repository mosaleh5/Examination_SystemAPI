using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Identity;

namespace Examination_System.Data
{
    public class Context
    : IdentityDbContext<
        User,
        IdentityRole<Guid>,
        Guid,
        IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {

        }


        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }

        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Instructor> Instructors{ get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<ExamAssignment> ExamAssignments { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .LogTo(Log => Debug.WriteLine(Log), LogLevel.Information)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
            }
        }

  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configure User with Guid primary key
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                
                entity.HasOne(u => u.Student)
                    .WithOne(s => s.User)
                    .HasForeignKey<Student>(s => s.Id)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.Instructor)
                    .WithOne(i => i.User)
                    .HasForeignKey<Instructor>(i => i.Id)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Students");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Instructor>(entity =>
            {
                entity.ToTable("Instructors");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.InstructorId)
                    .IsRequired();
                
                entity.HasOne(c => c.Instructor)
                    .WithMany(i => i.Courses)
                    .HasForeignKey(c => c.InstructorId);
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasOne(q => q.Instructor)
                    .WithMany(i => i.Questions)
                    .HasForeignKey(q => q.InstructorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(q => q.Course)
                    .WithMany(c => c.Questions)
                    .HasForeignKey(q => q.CourseId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });

        }
    }
}
