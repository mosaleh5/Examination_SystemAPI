using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Add this using directive
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Examination_System.Data
{
    public class Context : IdentityDbContext<User>
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
        public DbSet<User> Users { get; set; }
        public DbSet<Instructor> Instructors{ get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<ExamAssignment> ExamAssignments { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .LogTo(Log => Debug.WriteLine(Log), LogLevel.Information)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
        }

        // Add helper methods to ensure entities are tracked before saving
        public void AttachAndUpdate<T>(T entity) where T : class 
        {
            Entry(entity).State = EntityState.Modified;
        }

        public void AttachAndAdd<T>(T entity) where T : class
        {
            Entry(entity).State = EntityState.Added;
        }

        public void AttachAndDelete<T>(T entity) where T : class
        {
            Entry(entity).State = EntityState.Deleted;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
