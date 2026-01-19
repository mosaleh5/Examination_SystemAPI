using Examination_System.Models;
using Examination_System.Models.Enums;
using Microsoft.AspNetCore.Identity;
using static Examination_System.Models.Question;

namespace Examination_System.Data
{
    public static class SeedingData
    {
/*        public static List<User> GetUsers()
        {
            return
            [
                new User
                {
                    UserName = "ahmed.mohammed",
                    FirstName = "Ahmed",
                    LastName = "Mohammed",
                    Email = "ahmed.mohammed@example.com",
                    EmailConfirmed = true,
                    PhoneNumber = "+966512345678",
                },
                new User
                {
                    UserName = "fatima.ali",
                    FirstName = "Fatima",
                    LastName = "Ali",
                    Email = "fatima.ali@example.com",
                    EmailConfirmed = true,
                    PhoneNumber = "+966512345679",
                },
                new User
                {
                    UserName = "sara.hassan",
                    FirstName = "Sara",
                    LastName = "Hassan",
                    Email = "sara.hassan@example.com",
                    EmailConfirmed = true,
                    PhoneNumber = "+966512345680",
                },
                new User
                {
                    UserName = "mohammed.ibrahim",
                    FirstName = "Mohammed",
                    LastName = "Ibrahim",
                    Email = "mohammed.ibrahim@example.com",
                    EmailConfirmed = true,
                    PhoneNumber = "+966512345681",
                },
                new User
                {
                    UserName = "noor.abdullah",
                    FirstName = "Noor",
                    LastName = "Abdullah",
                    Email = "noor.abdullah@example.com",
                    EmailConfirmed = true,
                    PhoneNumber = "+966512345682",
                },
                new User
                {
                    UserName = "layla.saeed",
                    FirstName = "Layla",
                    LastName = "Saeed",
                    Email = "layla.saeed@example.com",
                    EmailConfirmed = true,
                    PhoneNumber = "+966512345683",
                },
                new User
                {
                    UserName = "omar.khalid",
                    FirstName = "Omar",
                    LastName = "Khalid",
                    Email = "omar.khalid@example.com",
                    EmailConfirmed = true,
                    PhoneNumber = "+966512345684",
                }
            ];
        }

        public static List<Instructor> GetInstructors(List<User> users)
        {
            return
            [
                new Instructor { Id = users[0].Id, IsDeleted = false,  },
                new Instructor { Id = users[1].Id, IsDeleted = false,  },
                new Instructor { Id = users[2].Id, IsDeleted = false,  }
            ];
        }

        public static List<Student> GetStudents(List<User> users)
        {
            return
            [
                new Student
                {
                    Id = users[3].Id,
                    Major = "Computer Science",
                    EnrollmentDate = new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false,
                },
                new Student
                {
                    Id = users[4].Id,
                    Major = "Information Technology",
                    EnrollmentDate = new DateTime(2024, 9, 2, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false,
                },
                new Student
                {
                    Id = users[5].Id,
                    Major = "Software Engineering",
                    EnrollmentDate = new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false,
                },
                new Student
                {
                    Id = users[6].Id,
                    Major = "Cybersecurity",
                    EnrollmentDate = new DateTime(2024, 9, 4, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false,
                }
            ];
        }

        public static List<Course> GetCourses(List<Instructor> instructors)
        {
            return
            [
                new Course
                {
                    Name = "Introduction to Programming",
                    Description = "Learn the fundamentals of programming using C#",
                    Hours = "40",
                    InstructorId = instructors[0].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Name = "Database Management",
                    Description = "Learn SQL and database design principles",
                    Hours = "35",
                    InstructorId = instructors[1].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Name = "Web Development",
                    Description = "Build modern web applications with ASP.NET Core",
                    Hours = "45",
                    InstructorId = instructors[2].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Name = "Data Structures and Algorithms",
                    Description = "Master fundamental data structures and algorithms",
                    Hours = "50",
                    InstructorId = instructors[0].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Name = "Software Engineering",
                    Description = "Learn software development lifecycle and best practices",
                    Hours = "38",
                    InstructorId = instructors[1].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                }
            ];
        }

        public static List<Exam> GetExams(List<Course> courses, List<Instructor> instructors)
        {
            return
            [
                new Exam
                {
                    Title = "Programming Midterm Exam",
                    Date = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc),
                    DurationInMinutes = 90,
                    Fullmark = 100,
                    ExamType = ExamType.Final,
                    QuestionsCount = 20,
                    CourseId = courses[0].Id,
                    InstructorId = instructors[0].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Exam
                {
                    Title = "Database Final Exam",
                    Date = new DateTime(2025, 2, 20, 14, 0, 0, DateTimeKind.Utc),
                    DurationInMinutes = 120,
                    Fullmark = 100,
                    ExamType = ExamType.Final,
                    QuestionsCount = 25,
                    CourseId = courses[1].Id,
                    InstructorId = instructors[1].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Exam
                {
                    Title = "Web Development Quiz 1",
                    Date = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Utc),
                    DurationInMinutes = 30,
                    Fullmark = 20,
                    ExamType = ExamType.Quiz,
                    QuestionsCount = 10,
                    CourseId = courses[2].Id,
                    InstructorId = instructors[2].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Exam
                {
                    Title = "Data Structures Midterm",
                    Date = new DateTime(2025, 1, 25, 11, 0, 0, DateTimeKind.Utc),
                    DurationInMinutes = 90,
                    Fullmark = 100,
                    ExamType = ExamType.Final,
                    QuestionsCount = 18,
                    CourseId = courses[3].Id,
                    InstructorId = instructors[0].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Exam
                {
                    Title = "Software Engineering Final",
                    Date = new DateTime(2025, 2, 28, 13, 0, 0, DateTimeKind.Utc),
                    DurationInMinutes = 120,
                    Fullmark = 100,
                    ExamType = ExamType.Final,
                    QuestionsCount = 22,
                    CourseId = courses[4].Id,
                    InstructorId = instructors[1].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Exam
                {
                    Title = "Programming Final Exam",
                    Date = new DateTime(2025, 2, 10, 10, 0, 0, DateTimeKind.Utc),
                    DurationInMinutes = 120,
                    Fullmark = 100,
                    ExamType = ExamType.Final,
                    QuestionsCount = 30,
                    CourseId = courses[0].Id,
                    InstructorId = instructors[0].Id,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                }
            ];
        }

        public static List<Question> GetQuestions(Guid instructorId, Guid courseId)
        {
            return new List<Question>
            {
                new Question { Title = "What is a variable in programming?", Mark = 5, Level = QuestionLevel.Simple, InstructorId = instructorId, CourseId = courseId, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Question { Title = "Explain the difference between value types and reference types in C#.", Mark = 10, Level = QuestionLevel.Medium, InstructorId = instructorId, CourseId = courseId, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Question { Title = "What is polymorphism and how is it implemented?", Mark = 15, Level = QuestionLevel.Hard, InstructorId = instructorId, CourseId = courseId, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Question { Title = "What is normalization in databases?", Mark = 5, Level = QuestionLevel.Simple, InstructorId = instructorId, CourseId = courseId, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Question { Title = "Write a SQL query to perform an INNER JOIN between two tables.", Mark = 15, Level = QuestionLevel.Hard, InstructorId = instructorId, CourseId = courseId, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Question { Title = "Explain ACID properties in database transactions.", Mark = 10, Level = QuestionLevel.Medium, InstructorId = instructorId, CourseId = courseId, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Question { Title = "What is the MVC pattern?", Mark = 5, Level = QuestionLevel.Simple, InstructorId = instructorId, CourseId = courseId, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Question { Title = "Explain RESTful API design principles.", Mark = 10, Level = QuestionLevel.Medium, InstructorId = instructorId, CourseId = courseId, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Question { Title = "What is the difference between authentication and authorization?", Mark = 10, Level = QuestionLevel.Medium, InstructorId = instructorId, CourseId = courseId, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Question { Title = "Implement a binary search algorithm.", Mark = 15, Level = QuestionLevel.Hard, InstructorId = instructorId, CourseId = courseId, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Question { Title = "Explain the concept of inheritance in OOP.", Mark = 10, Level = QuestionLevel.Medium, InstructorId = instructorId, CourseId = courseId, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Question { Title = "What is a foreign key?", Mark = 5, Level = QuestionLevel.Simple, InstructorId = instructorId, CourseId = courseId, IsDeleted = false, CreatedAt = DateTime.UtcNow }
            };
        }

        public static List<Choice> GetChoices(List<Question> questions)
        {
            return
            [
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

                // Question 2: Polymorphism
                new Choice { Text = "Ability of objects to take on many forms", IsCorrect = true, QuestionId = questions[2].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "A type of inheritance only", IsCorrect = false, QuestionId = questions[2].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

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

                // Question 5: ACID properties
                new Choice { Text = "Atomicity, Consistency, Isolation, Durability", IsCorrect = true, QuestionId = questions[5].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Add, Create, Insert, Delete", IsCorrect = false, QuestionId = questions[5].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Question 6: MVC Pattern
                new Choice { Text = "Model-View-Controller architectural pattern", IsCorrect = true, QuestionId = questions[6].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Multiple Virtual Connections", IsCorrect = false, QuestionId = questions[6].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Main Variable Controller", IsCorrect = false, QuestionId = questions[6].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Memory View Cache", IsCorrect = false, QuestionId = questions[6].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Question 7: RESTful API
                new Choice { Text = "Stateless, resource-based, uses HTTP verbs", IsCorrect = true, QuestionId = questions[7].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Stateful, action-based, uses RPC calls", IsCorrect = false, QuestionId = questions[7].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Question 8: Authentication vs Authorization
                new Choice { Text = "Authentication verifies identity, authorization verifies permissions", IsCorrect = true, QuestionId = questions[8].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "They are the same thing", IsCorrect = false, QuestionId = questions[8].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Authorization happens before authentication", IsCorrect = false, QuestionId = questions[8].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Authentication is only used in web applications", IsCorrect = false, QuestionId = questions[8].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Question 9: Binary search
                new Choice { Text = "Divide and conquer search in sorted array", IsCorrect = true, QuestionId = questions[9].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Linear search from the beginning", IsCorrect = false, QuestionId = questions[9].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Question 10: Inheritance
                new Choice { Text = "Mechanism to derive new classes from existing ones", IsCorrect = true, QuestionId = questions[10].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "A way to hide implementation details", IsCorrect = false, QuestionId = questions[10].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Question 11: Foreign key
                new Choice { Text = "A field that links to the primary key of another table", IsCorrect = true, QuestionId = questions[11].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "A unique identifier in a table", IsCorrect = false, QuestionId = questions[11].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "A type of index", IsCorrect = false, QuestionId = questions[11].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "A database constraint", IsCorrect = false, QuestionId = questions[11].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow }
            ];
        }

        public static List<ExamQuestion> GetExamQuestions(List<Exam> exams, List<Question> questions)
        {
            return
            [
                new ExamQuestion { ExamId = exams[0].Id, QuestionId = questions[0].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { ExamId = exams[0].Id, QuestionId = questions[1].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { ExamId = exams[0].Id, QuestionId = questions[2].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

                new ExamQuestion { ExamId = exams[1].Id, QuestionId = questions[3].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { ExamId = exams[1].Id, QuestionId = questions[4].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { ExamId = exams[1].Id, QuestionId = questions[5].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { ExamId = exams[1].Id, QuestionId = questions[11].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

                new ExamQuestion { ExamId = exams[2].Id, QuestionId = questions[6].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { ExamId = exams[2].Id, QuestionId = questions[7].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { ExamId = exams[2].Id, QuestionId = questions[8].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

                new ExamQuestion { ExamId = exams[3].Id, QuestionId = questions[9].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { ExamId = exams[3].Id, QuestionId = questions[0].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { ExamId = exams[3].Id, QuestionId = questions[10].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

                new ExamQuestion { ExamId = exams[4].Id, QuestionId = questions[7].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { ExamId = exams[4].Id, QuestionId = questions[5].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { ExamId = exams[4].Id, QuestionId = questions[10].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },

                new ExamQuestion { ExamId = exams[5].Id, QuestionId = questions[2].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { ExamId = exams[5].Id, QuestionId = questions[9].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { ExamId = exams[5].Id, QuestionId = questions[10].Id, IsDeleted = false, CreatedAt = DateTime.UtcNow }
            ];
        }

        public static List<CourseEnrollment> GetCourseEnrollments(List<Student> students, List<Course> courses)
        {
            return
            [
                new CourseEnrollment { StudentId = students[0].Id, CourseId = courses[0].Id, EnrollmentDate = new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { StudentId = students[0].Id, CourseId = courses[1].Id, EnrollmentDate = new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { StudentId = students[0].Id, CourseId = courses[3].Id, EnrollmentDate = new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },

                new CourseEnrollment { StudentId = students[1].Id, CourseId = courses[0].Id, EnrollmentDate = new DateTime(2024, 9, 2, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { StudentId = students[1].Id, CourseId = courses[1].Id, EnrollmentDate = new DateTime(2024, 9, 2, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { StudentId = students[1].Id, CourseId = courses[2].Id, EnrollmentDate = new DateTime(2024, 9, 2, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { StudentId = students[1].Id, CourseId = courses[4].Id, EnrollmentDate = new DateTime(2024, 9, 2, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },

                new CourseEnrollment { StudentId = students[2].Id, CourseId = courses[0].Id, EnrollmentDate = new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { StudentId = students[2].Id, CourseId = courses[1].Id, EnrollmentDate = new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { StudentId = students[2].Id, CourseId = courses[2].Id, EnrollmentDate = new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { StudentId = students[2].Id, CourseId = courses[4].Id, EnrollmentDate = new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },

                new CourseEnrollment { StudentId = students[3].Id, CourseId = courses[0].Id, EnrollmentDate = new DateTime(2024, 9, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { StudentId = students[3].Id, CourseId = courses[2].Id, EnrollmentDate = new DateTime(2024, 9, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { StudentId = students[3].Id, CourseId = courses[3].Id, EnrollmentDate = new DateTime(2024, 9, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow }
            ];
        }*/

        public static async Task SeedAdminUser(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }

            var adminUser = new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@exam.com",
                UserName = "admin@exam.com",
                EmailConfirmed = true
            };

            var existingUser = await userManager.FindByEmailAsync(adminUser.Email);
            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
