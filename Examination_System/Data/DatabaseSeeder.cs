using Examination_System.Models;

namespace Examination_System.Data
{
    public class DatabaseSeeder
    {
        private readonly Context _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(Context context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Starting database seeding...");

                // Check if database already has data
                if (_context.Courses.Any() || _context.Students.Any() || _context.Instructors.Any())
                {
                    _logger.LogWarning("Database already contains data. Skipping seed.");
                    return;
                }

                var users = SeedData.GetUsers();
                _context.Users.AddRange(users);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {users.Count} users");

                var instructors = SeedData.GetInstructors(users);
                _context.Instructors.AddRange(instructors);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {instructors.Count} instructors");

                var students = SeedData.GetStudents(users);
                _context.Students.AddRange(students);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {students.Count} students");

                var courses = SeedData.GetCourses(instructors);
                _context.Courses.AddRange(courses);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {courses.Count} courses");

                var exams = SeedData.GetExams(courses, instructors);
                _context.Exams.AddRange(exams);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {exams.Count} exams");

                var questions = SeedData.GetQuestions(instructors);
                _context.Questions.AddRange(questions);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {questions.Count} questions");

                var choices = SeedData.GetChoices(questions);
                _context.Choices.AddRange(choices);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {choices.Count} choices");

                var examQuestions = SeedData.GetExamQuestions(exams, questions);
                _context.ExamQuestions.AddRange(examQuestions);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {examQuestions.Count} exam questions");

                var enrollments = SeedData.GetCourseEnrollments(students, courses);
                _context.CourseEnrollments.AddRange(enrollments);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {enrollments.Count} course enrollments");

                var examAssignments = SeedData.GetExamAssignments(students, exams);
                _context.ExamAssignments.AddRange(examAssignments);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {examAssignments.Count} exam assignments");

                _logger.LogInformation("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during database seeding: {ex.Message}");
                throw;
            }
        }
    }
}