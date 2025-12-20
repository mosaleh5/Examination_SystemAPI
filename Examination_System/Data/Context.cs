using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Examination_System.Data
{
    public class Context : DbContext
    {
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
            optionsBuilder.UseSqlServer("Server=.;Database=ExaminationSystemDB;Trusted_Connection=True;TrustServerCertificate=True;")
                .LogTo(Log => Debug.WriteLine(Log), LogLevel.Information)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
            
            
        }
    }
}
