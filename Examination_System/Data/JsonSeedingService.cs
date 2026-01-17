using System.Text.Json;
using Examination_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Examination_System.Models.Question;
using static Examination_System.Models.Exam;
using  Examination_System.Models.Enums;
namespace Examination_System.Data
{
    public class JsonSeedingService
    {
       
        public async Task SeedFromJsonAsync(
            Context context,
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ILogger<JsonSeedingService> logger)
        {
           
            try
            {
                var seedDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData");

                // 1. Seed users (instructors and students)
                var users = await LoadUsersFromJsonAsync(seedDataPath);
                await SeedUsersAsync(userManager, users, logger, context);

                // 2. Get instructors and students from database
                var instructors = await context.Instructors.ToListAsync();
                var students = await context.Students.ToListAsync();

                // 3. Seed courses
                var courses = await LoadCoursesFromJsonAsync(seedDataPath, instructors);
                await context.Courses.AddRangeAsync(courses);
                await context.SaveChangesAsync();
                logger.LogInformation($"Seeded {courses.Count} courses");

                // 4. Seed exams
                var exams = await LoadExamsFromJsonAsync(seedDataPath, courses, instructors);
                await context.Exams.AddRangeAsync(exams);
                await context.SaveChangesAsync();
                logger.LogInformation($"Seeded {exams.Count} exams");

                // 5. Seed questions
                var questions = await LoadQuestionsFromJsonAsync(seedDataPath, instructors, courses);
                await context.Questions.AddRangeAsync(questions);
                await context.SaveChangesAsync();
                logger.LogInformation($"Seeded {questions.Count} questions");

                // 6. Seed choices
                var choices = await LoadChoicesFromJsonAsync(seedDataPath, questions);
                await context.Choices.AddRangeAsync(choices);
                await context.SaveChangesAsync();
                logger.LogInformation($"Seeded {choices.Count} choices");

                // 7. Seed exam questions
                var examQuestions = await LoadExamQuestionsFromJsonAsync(seedDataPath, exams, questions);
                await context.ExamQuestions.AddRangeAsync(examQuestions);
                await context.SaveChangesAsync();
                logger.LogInformation($"Seeded {examQuestions.Count} exam-question mappings");

                // 8. Seed course enrollments
                var enrollments = await LoadCourseEnrollmentsFromJsonAsync(seedDataPath, students, courses);
                await context.CourseEnrollments.AddRangeAsync(enrollments);
                await context.SaveChangesAsync();
                logger.LogInformation($"Seeded {enrollments.Count} course enrollments");

                // 9. Seed admin user
                await SeedingData.SeedAdminUser(userManager, roleManager);

                logger.LogInformation("All seed data loaded successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error during JSON seeding: {ex.Message}");
                throw;
            }
        }

        private static async Task<List<(User user, string discriminator, string? major, DateTime? enrollmentDate)>> LoadUsersFromJsonAsync(string seedDataPath)
        {
            var filePath = Path.Combine(seedDataPath, "users.json");
            var json = await File.ReadAllTextAsync(filePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement.GetProperty("users");

            var users = new List<(User, string, string?, DateTime?)>();
            foreach (var item in root.EnumerateArray())
            {
                var user = new User
                {
                    UserName = item.GetProperty("userName").GetString() ?? string.Empty,
                    FirstName = item.GetProperty("firstName").GetString() ?? string.Empty,
                    LastName = item.GetProperty("lastName").GetString() ?? string.Empty,
                    Email = item.GetProperty("email").GetString() ?? string.Empty,
                    EmailConfirmed = item.GetProperty("emailConfirmed").GetBoolean(),
                    PhoneNumber = item.GetProperty("phoneNumber").GetString() ?? string.Empty
                };

                var discriminator = item.GetProperty("discriminator").GetString() ?? string.Empty;
                var major = item.TryGetProperty("major", out var majorElement) ? majorElement.GetString() : null;
                var enrollmentDate = item.TryGetProperty("enrollmentDate", out var dateElement) ? dateElement.GetDateTime() : (DateTime?)null;

                users.Add((user, discriminator, major, enrollmentDate));
               
            }

            return users;
        }

        private static async Task SeedUsersAsync(
            UserManager<User> userManager,
            List<(User user, string discriminator, string? major, DateTime? enrollmentDate)> users,
            ILogger<JsonSeedingService> logger,
            Context context)
        {
            var instructors = new List<Instructor>();
            var students = new List<Student>();

            foreach (var (user, discriminator, major, enrollmentDate) in users)
            {
                var result = await userManager.CreateAsync(user, "DefaultPassword@123");
                if (!result.Succeeded)
                {
                    logger.LogError($"Failed to create user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    continue;
                }

                if (discriminator == "Instructor")
                {
                    var InstructorUser = new Instructor { Id = user.Id };
                    instructors.Add(InstructorUser);
                    await userManager.AddToRoleAsync(user, "Instructor");
                }
                else if (discriminator == "Student")
                {
                    students.Add(new Student
                    {
                        Id = user.Id,
                        Major = major ?? string.Empty,
                        EnrollmentDate = enrollmentDate ?? DateTime.UtcNow
                    });
                    await userManager.AddToRoleAsync(user, "Student");

                }
            }

            context.Instructors.AddRange(instructors);
            context.Students.AddRange(students);
            await context.SaveChangesAsync();

            logger.LogInformation($"Seeded {instructors.Count} instructors and {students.Count} students");
        }

        private static async Task<List<Course>> LoadCoursesFromJsonAsync(string seedDataPath, List<Instructor> instructors)
        {
            var filePath = Path.Combine(seedDataPath, "courses.json");
            var json = await File.ReadAllTextAsync(filePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement.GetProperty("courses");

            var courses = new List<Course>();
            var courseIndex = 0;
            foreach (var item in root.EnumerateArray())
            {
                var instructorIndex = item.GetProperty("instructorIndex").GetInt32();
                courses.Add(new Course
                {
                    Name = item.GetProperty("name").GetString() ?? string.Empty,
                    Description = item.GetProperty("description").GetString() ?? string.Empty,
                    Hours = item.GetProperty("hours").GetString() ?? string.Empty,
                    InstructorId = instructors[instructorIndex].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                });
                courseIndex++;
            }

            return courses;
        }

        private static async Task<List<Exam>> LoadExamsFromJsonAsync(string seedDataPath, List<Course> courses, List<Instructor> instructors)
        {
            var filePath = Path.Combine(seedDataPath, "exams.json");
            var json = await File.ReadAllTextAsync(filePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement.GetProperty("exams");

            var exams = new List<Exam>();
            foreach (var item in root.EnumerateArray())
            {
                var courseIndex = item.GetProperty("courseIndex").GetInt32();
                var instructorIndex = item.GetProperty("instructorIndex").GetInt32();

                exams.Add(new Exam
                {
                    Title = item.GetProperty("title").GetString() ?? string.Empty,
                    Date = item.GetProperty("date").GetDateTime(),
                    DurationInMinutes = item.GetProperty("durationInMinutes").GetInt32(),
                    Fullmark = item.GetProperty("fullmark").GetInt32(),
                    ExamType = (Examination_System.Models.Enums.ExamType)item.GetProperty("examType").GetInt32(),
                    QuestionsCount = item.GetProperty("questionsCount").GetInt32(),
                    CourseId = courses[courseIndex].Id,
                    InstructorId = instructors[instructorIndex].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            return exams;
        }

        private static async Task<List<Question>> LoadQuestionsFromJsonAsync(string seedDataPath, List<Instructor> instructors, List<Course> courses)
        {
            var filePath = Path.Combine(seedDataPath, "questions.json");
            var json = await File.ReadAllTextAsync(filePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement.GetProperty("questions");

            var questions = new List<Question>();
            foreach (var item in root.EnumerateArray())
            {
                var instructorIndex = item.GetProperty("instructorIndex").GetInt32();
                var courseIndex = item.GetProperty("courseIndex").GetInt32();

                questions.Add(new Question
                {
                    Title = item.GetProperty("title").GetString() ?? string.Empty,
                    Mark = item.GetProperty("mark").GetInt32(),
                    Level = (QuestionLevel)item.GetProperty("level").GetInt32(),
                    InstructorId = instructors[instructorIndex].Id,
                    CourseId = courses[courseIndex].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            return questions;
        }

        private static async Task<List<Choice>> LoadChoicesFromJsonAsync(string seedDataPath, List<Question> questions)
        {
            var filePath = Path.Combine(seedDataPath, "choices.json");
            var json = await File.ReadAllTextAsync(filePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement.GetProperty("choices");

            var choices = new List<Choice>();
            foreach (var item in root.EnumerateArray())
            {
                var questionIndex = item.GetProperty("questionIndex").GetInt32();
                choices.Add(new Choice
                {
                    Text = item.GetProperty("text").GetString() ?? string.Empty,
                    IsCorrect = item.GetProperty("isCorrect").GetBoolean(),
                    QuestionId = questions[questionIndex].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            return choices;
        }

        private static async Task<List<ExamQuestion>> LoadExamQuestionsFromJsonAsync(string seedDataPath, List<Exam> exams, List<Question> questions)
        {
            var filePath = Path.Combine(seedDataPath, "examQuestions.json");
            var json = await File.ReadAllTextAsync(filePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement.GetProperty("examQuestions");

            var examQuestions = new List<ExamQuestion>();
            foreach (var item in root.EnumerateArray())
            {
                var examIndex = item.GetProperty("examIndex").GetInt32();
                var questionIndices = item.GetProperty("questionIndices").EnumerateArray().Select(x => x.GetInt32()).ToList();

                foreach (var questionIndex in questionIndices)
                {
                    examQuestions.Add(new ExamQuestion
                    {
                        ExamId = exams[examIndex].Id,
                        QuestionId = questions[questionIndex].Id,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            return examQuestions;
        }

        private static async Task<List<CourseEnrollment>> LoadCourseEnrollmentsFromJsonAsync(string seedDataPath, List<Student> students, List<Course> courses)
        {
            var filePath = Path.Combine(seedDataPath, "courseEnrollments.json");
            var json = await File.ReadAllTextAsync(filePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement.GetProperty("enrollments");

            var enrollments = new List<CourseEnrollment>();
            foreach (var item in root.EnumerateArray())
            {
                var studentIndex = item.GetProperty("studentIndex").GetInt32();
                var courseIndex = item.GetProperty("courseIndex").GetInt32();

                enrollments.Add(new CourseEnrollment
                {
                    StudentId = students[studentIndex].Id,
                    CourseId = courses[courseIndex].Id,
                    EnrollmentDate = item.GetProperty("enrollmentDate").GetDateTime(),
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            return enrollments;
        }
    }
}
