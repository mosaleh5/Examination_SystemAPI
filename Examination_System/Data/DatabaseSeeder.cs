using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using static Examination_System.Models.Question;

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
        public async static Task SeedAsync(Context _context, ILogger<DatabaseSeeder> _logger)
        {
            try
            {
                _logger.LogInformation("Starting database seeding...");

                // Check if database already has data
                if (_context.Questions.Any())
                {
                    _logger.LogWarning("Database already contains questions. Skipping seed.");
                    return;
                }

                // Get the first instructor from the database (seeded via UserDataSeeding)
                var firstInstructor = await _context.Instructors.FirstOrDefaultAsync();
                if (firstInstructor == null)
                {
                    _logger.LogError("No instructors found in database. Cannot seed questions.");
                    return;
                }

                // Get the first course from the database
                var firstCourse = await _context.Courses.FirstOrDefaultAsync();
                if (firstCourse == null)
                {
                    _logger.LogError("No courses found in database. Cannot seed questions.");
                    return;
                }

                // Create questions with actual instructor and course IDs
                var questions = new List<Question>
        {
            new Question { Title = "What is a variable in programming?", Mark = 5, Level = QuestionLevel.Simple, InstructorId = firstInstructor.Id, CourseId = firstCourse.Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Question { Title = "Explain the difference between value types and reference types in C#.", Mark = 10, Level = QuestionLevel.Medium, InstructorId = firstInstructor.Id, CourseId = firstCourse.Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Question { Title = "What is polymorphism and how is it implemented?", Mark = 15, Level = QuestionLevel.Hard, InstructorId = firstInstructor.Id, CourseId = firstCourse.Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Question { Title = "What is normalization in databases?", Mark = 5, Level = QuestionLevel.Simple, InstructorId = firstInstructor.Id, CourseId = firstCourse.Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Question { Title = "Write a SQL query to perform an INNER JOIN between two tables.", Mark = 15, Level = QuestionLevel.Hard, InstructorId = firstInstructor.Id, CourseId = firstCourse.Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Question { Title = "Explain ACID properties in database transactions.", Mark = 10, Level = QuestionLevel.Medium, InstructorId = firstInstructor.Id, CourseId = firstCourse.Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Question { Title = "What is the MVC pattern?", Mark = 5, Level = QuestionLevel.Simple, InstructorId = firstInstructor.Id, CourseId = firstCourse.Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Question { Title = "Explain RESTful API design principles.", Mark = 10, Level = QuestionLevel.Medium, InstructorId = firstInstructor.Id, CourseId = firstCourse.Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Question { Title = "What is the difference between authentication and authorization?", Mark = 10, Level = QuestionLevel.Medium, InstructorId = firstInstructor.Id, CourseId = firstCourse.Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Question { Title = "Implement a binary search algorithm.", Mark = 15, Level = QuestionLevel.Hard, InstructorId = firstInstructor.Id, CourseId = firstCourse.Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Question { Title = "Explain the concept of inheritance in OOP.", Mark = 10, Level = QuestionLevel.Medium, InstructorId = firstInstructor.Id, CourseId = firstCourse.Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Question { Title = "What is a foreign key?", Mark = 5, Level = QuestionLevel.Simple, InstructorId = firstInstructor.Id, CourseId = firstCourse.Id, IsDeleted = false, CreatedAt = DateTime.UtcNow }
        };

                _context.Questions.AddRange(questions);
                await _context.SaveChangesAsync(); // ✅ SAVE QUESTIONS FIRST TO GET IDs
                _logger.LogInformation($"Seeded {questions.Count} questions");

                // Now create choices using the saved question IDs (NOT navigation properties)
                var choices = new List<Choice>
        {
            // Question 0: What is a variable
            new Choice { Text = "A named storage location in memory", IsCorrect = true, QuestionId = questions[0].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "A type of loop", IsCorrect = false, QuestionId = questions[0].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "A function definition", IsCorrect = false, QuestionId = questions[0].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "A class member", IsCorrect = false, QuestionId = questions[0].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

            // Question 1: Value vs Reference types
            new Choice { Text = "Value types store data directly, reference types store a reference to data", IsCorrect = true, QuestionId = questions[1].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "They are exactly the same", IsCorrect = false, QuestionId = questions[1].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "Reference types are always faster", IsCorrect = false, QuestionId = questions[1].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "Value types cannot be modified after creation", IsCorrect = false, QuestionId = questions[1].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

            // Question 3: Normalization
            new Choice { Text = "Process of organizing data to reduce redundancy", IsCorrect = true, QuestionId = questions[3].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "Creating database indexes", IsCorrect = false, QuestionId = questions[3].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "Backing up database data", IsCorrect = false, QuestionId = questions[3].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "Encrypting database content", IsCorrect = false, QuestionId = questions[3].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

            // Question 4: SQL JOIN
            new Choice { Text = "SELECT * FROM TableA INNER JOIN TableB ON TableA.id = TableB.id", IsCorrect = true, QuestionId = questions[4].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "SELECT * FROM TableA, TableB", IsCorrect = false, QuestionId = questions[4].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "JOIN TableA WITH TableB", IsCorrect = false, QuestionId = questions[4].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "MERGE TableA AND TableB", IsCorrect = false, QuestionId = questions[4].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

            // Question 6: MVC Pattern
            new Choice { Text = "Model-View-Controller architectural pattern", IsCorrect = true, QuestionId = questions[6].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "Multiple Virtual Connections", IsCorrect = false, QuestionId = questions[6].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "Main Variable Controller", IsCorrect = false, QuestionId = questions[6].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "Memory View Cache", IsCorrect = false, QuestionId = questions[6].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

            // Question 8: Authentication vs Authorization
            new Choice { Text = "Authentication verifies identity, authorization verifies permissions", IsCorrect = true, QuestionId = questions[8].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "They are the same thing", IsCorrect = false, QuestionId = questions[8].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "Authorization happens before authentication", IsCorrect = false, QuestionId = questions[8].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "Authentication is only used in web applications", IsCorrect = false, QuestionId = questions[8].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

            // Question 11: Foreign key
            new Choice { Text = "A field that links to the primary key of another table", IsCorrect = true, QuestionId = questions[11].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "A unique identifier in a table", IsCorrect = false, QuestionId = questions[11].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "A type of index", IsCorrect = false, QuestionId = questions[11].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
            new Choice { Text = "A database constraint", IsCorrect = false, QuestionId = questions[11].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow }
        };

                _context.Choices.AddRange(choices);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {choices.Count} choices");

                _logger.LogInformation("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during database seeding: {ex.Message}");
                _logger.LogError($"Inner exception: {ex.InnerException?.Message}");
                throw;
            }
        }
        public async static Task SeedingAsync(Context _context , ILogger<DatabaseSeeder> _logger)
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

     /*   var users = SeedData.GetUsers();
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
        _logger.LogInformation($"Seeded {exams.Count} exams");*/

        var questions = SeedingData.GetQuestions();
        _context.Questions.AddRange(questions);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Seeded {questions.Count} questions");

        var choices = SeedingData.GetChoices(questions);
        _context.Choices.AddRange(choices);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Seeded {choices.Count} choices");

    /*    var examQuestions = SeedData.GetExamQuestions(exams, questions);
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
        _logger.LogInformation($"Seeded {examAssignments.Count} exam assignments");*/

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