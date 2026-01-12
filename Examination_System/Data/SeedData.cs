using Examination_System.Models;
using Microsoft.AspNetCore.Identity;
using static Examination_System.Models.Question;

namespace Examination_System.Data
{
    public static class SeedingData
    {
      /*  public static List<User> GetUsers()
        {
            return
            [
                new User
                {
                    UserName = "ahmed.mohammed",
                    FirstName = "Ahmed",
                    LastName = "Mohammed",
                    Email = "ahmed.mohammed@example.com",
                    Password = "Password@123",
                    Phone = "+966512345678",
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 12, 13, 9, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserName = "fatima.ali",
                    FirstName = "Fatima",
                    LastName = "Ali",
                    Email = "fatima.ali@example.com",
                    Password = "Password@456",
                    Phone = "+966512345679",
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 12, 13, 9, 5, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserName = "sara.hassan",
                    FirstName = "Sara",
                    LastName = "Hassan",
                    Email = "sara.hassan@example.com",
                    Password = "Password@789",
                    Phone = "+966512345680",
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 12, 13, 9, 10, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserName = "mohammed.ibrahim",
                    FirstName = "Mohammed",
                    LastName = "Ibrahim",
                    Email = "mohammed.ibrahim@example.com",
                    Password = "Student@123",
                    Phone = "+966512345681",
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 12, 13, 9, 15, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserName = "noor.abdullah",
                    FirstName = "Noor",
                    LastName = "Abdullah",
                    Email = "noor.abdullah@example.com",
                    Password = "Student@456",
                    Phone = "+966512345682",
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 12, 13, 9, 20, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserName = "layla.saeed",
                    FirstName = "Layla",
                    LastName = "Saeed",
                    Email = "layla.saeed@example.com",
                    Password = "Student@789",
                    Phone = "+966512345683",
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 12, 13, 9, 25, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserName = "omar.khalid",
                    FirstName = "Omar",
                    LastName = "Khalid",
                    Email = "omar.khalid@example.com",
                    Password = "Student@101",
                    Phone = "+966512345684",
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 12, 13, 9, 30, 0, DateTimeKind.Utc)
                }
            ];
        }

        public static List<Instructor> GetInstructors(List<User> users)
        {
            return
            [
                new Instructor { User = users[0], IsDeleted = false, CreatedAt = users[0].CreatedAt },
                new Instructor { User = users[1], IsDeleted = false, CreatedAt = users[1].CreatedAt },
                new Instructor { User = users[2], IsDeleted = false, CreatedAt = users[2].CreatedAt }
            ];
        }

        public static List<Student> GetStudents(List<User> users)
        {
            return
            [
                new Student
                {
                    User = users[3],
                    Major = "Computer Science",
                    EnrollmentDate = new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false,
                    CreatedAt = users[3].CreatedAt
                },
                new Student
                {
                    User = users[4],
                    Major = "Information Technology",
                    EnrollmentDate = new DateTime(2024, 9, 2, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false,
                    CreatedAt = users[4].CreatedAt
                },
                new Student
                {
                    User = users[5],
                    Major = "Software Engineering",
                    EnrollmentDate = new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false,
                    CreatedAt = users[5].CreatedAt
                },
                new Student
                {
                    User = users[6],
                    Major = "Cybersecurity",
                    EnrollmentDate = new DateTime(2024, 9, 4, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false,
                    CreatedAt = users[6].CreatedAt
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
                    Instructor = instructors[0],
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Name = "Database Management",
                    Description = "Learn SQL and database design principles",
                    Hours = "35",
                    Instructor = instructors[1],
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Name = "Web Development",
                    Description = "Build modern web applications with ASP.NET Core",
                    Hours = "45",
                    Instructor = instructors[2],
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Name = "Data Structures and Algorithms",
                    Description = "Master fundamental data structures and algorithms",
                    Hours = "50",
                    Instructor = instructors[0],
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Name = "Software Engineering",
                    Description = "Learn software development lifecycle and best practices",
                    Hours = "38",
                    Instructor = instructors[1],
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
                    DurationMinutes = 90,
                    Fullmark = 100,
                    ExamType = ExamType.Final,
                    QuestionsCount = 20,
                    Course = courses[0],
                    Instructor = instructors[0],
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Exam
                {
                    Title = "Database Final Exam",
                    Date = new DateTime(2025, 2, 20, 14, 0, 0, DateTimeKind.Utc),
                    DurationMinutes = 120,
                    Fullmark = 100,
                    ExamType = ExamType.Final,
                    QuestionsCount = 25,
                    Course = courses[1],
                    Instructor = instructors[1],
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Exam
                {
                    Title = "Web Development Quiz 1",
                    Date = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Utc),
                    DurationMinutes = 30,
                    Fullmark = 20,
                    ExamType = ExamType.Quiz,
                    QuestionsCount = 10,
                    Course = courses[2],
                    Instructor = instructors[2],
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Exam
                {
                    Title = "Data Structures Midterm",
                    Date = new DateTime(2025, 1, 25, 11, 0, 0, DateTimeKind.Utc),
                    DurationMinutes = 90,
                    Fullmark = 100,
                    ExamType = ExamType.Final,
                    QuestionsCount = 18,
                    Course = courses[3],
                    Instructor = instructors[0],
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Exam
                {
                    Title = "Software Engineering Final",
                    Date = new DateTime(2025, 2, 28, 13, 0, 0, DateTimeKind.Utc),
                    DurationMinutes = 120,
                    Fullmark = 100,
                    ExamType = ExamType.Final,
                    QuestionsCount = 22,
                    Course = courses[4],
                    Instructor = instructors[1],
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Exam
                {
                    Title = "Programming Final Exam",
                    Date = new DateTime(2025, 2, 10, 10, 0, 0, DateTimeKind.Utc),
                    DurationMinutes = 120,
                    Fullmark = 100,
                    ExamType = ExamType.Final,
                    QuestionsCount = 30,
                    Course = courses[0],
                    Instructor = instructors[0],
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                }
            ];
        }
*/
        public static List<Question> GetQuestions()
        {
            return new List<Question>
    {
        new Question { Title = "What is a variable in programming?", mark = 5, Level = QuestionLevel.Simple, InstructorId = Guid.Parse("f6ccfb9f-aee9-4151-91b0-f25daccd2831"), CourseId = new Guid("00000000-0000-0000-0000-000000000009"), IsDeleted = false, CreatedAt = DateTime.UtcNow },
        new Question { Title = "Explain the difference between value types and reference types in C#.", mark = 10, Level = QuestionLevel.Medium, InstructorId = Guid.Parse("f6ccfb9f-aee9-4151-91b0-f25daccd2831"), CourseId = new Guid("00000000-0000-0000-0000-000000000009"), IsDeleted = false, CreatedAt = DateTime.UtcNow },
        new Question { Title = "What is polymorphism and how is it implemented?", mark = 15, Level = QuestionLevel.Hard, InstructorId = Guid.Parse("f6ccfb9f-aee9-4151-91b0-f25daccd2831"), CourseId = new Guid("00000000-0000-0000-0000-000000000009"), IsDeleted = false, CreatedAt = DateTime.UtcNow },
        new Question { Title = "What is normalization in databases?", mark = 5, Level = QuestionLevel.Simple, InstructorId = Guid.Parse("f6ccfb9f-aee9-4151-91b0-f25daccd2831"), CourseId = new Guid("00000000-0000-0000-0000-000000000009"), IsDeleted = false, CreatedAt = DateTime.UtcNow },
        new Question { Title = "Write a SQL query to perform an INNER JOIN between two tables.", mark = 15, Level = QuestionLevel.Hard, InstructorId = Guid.Parse("f6ccfb9f-aee9-4151-91b0-f25daccd2831"), CourseId = new Guid("00000000-0000-0000-0000-000000000009"), IsDeleted = false, CreatedAt = DateTime.UtcNow },
        new Question { Title = "Explain ACID properties in database transactions.", mark = 10, Level = QuestionLevel.Medium, InstructorId = Guid.Parse("f6ccfb9f-aee9-4151-91b0-f25daccd2831"), CourseId = new Guid("00000000-0000-0000-0000-000000000009"), IsDeleted = false, CreatedAt = DateTime.UtcNow },
        new Question { Title = "What is the MVC pattern?", mark = 5, Level = QuestionLevel.Simple, InstructorId = Guid.Parse("f6ccfb9f-aee9-4151-91b0-f25daccd2831"), CourseId = new Guid("00000000-0000-0000-0000-000000000009"), IsDeleted = false, CreatedAt = DateTime.UtcNow },
        new Question { Title = "Explain RESTful API design principles.", mark = 10, Level = QuestionLevel.Medium, InstructorId = Guid.Parse("f6ccfb9f-aee9-4151-91b0-f25daccd2831"), CourseId = new Guid("00000000-0000-0000-0000-000000000009"), IsDeleted = false, CreatedAt = DateTime.UtcNow },
        new Question { Title = "What is the difference between authentication and authorization?", mark = 10, Level = QuestionLevel.Medium, InstructorId = Guid.Parse("f6ccfb9f-aee9-4151-91b0-f25daccd2831"), CourseId = new Guid("00000000-0000-0000-0000-000000000009"), IsDeleted = false, CreatedAt = DateTime.UtcNow },
        new Question { Title = "Implement a binary search algorithm.", mark = 15, Level = QuestionLevel.Hard, InstructorId = Guid.Parse("f6ccfb9f-aee9-4151-91b0-f25daccd2831"), CourseId = new Guid("00000000-0000-0000-0000-000000000009"), IsDeleted = false, CreatedAt = DateTime.UtcNow },
        new Question { Title = "Explain the concept of inheritance in OOP.", mark = 10, Level = QuestionLevel.Medium, InstructorId = Guid.Parse("f6ccfb9f-aee9-4151-91b0-f25daccd2831"), CourseId = new Guid("00000000-0000-0000-0000-000000000009"), IsDeleted = false, CreatedAt = DateTime.UtcNow },
        new Question { Title = "What is a foreign key?", mark = 5, Level = QuestionLevel.Simple, InstructorId = Guid.Parse("f6ccfb9f-aee9-4151-91b0-f25daccd2831"), CourseId = new Guid("00000000-0000-0000-0000-000000000009"), IsDeleted = false, CreatedAt = DateTime.UtcNow }
    };
        }

        public static List<Choice> GetChoices(List<Question> questions)
        {
            return
            [
                // Question 0: What is a variable
                new Choice { Text = "A named storage location in memory", IsCorrect = true, Question = questions[0], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "A type of loop", IsCorrect = false, Question = questions[0], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "A function definition", IsCorrect = false, Question = questions[0], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "A class member", IsCorrect = false, Question = questions[0], IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Question 1: Value vs Reference types
                new Choice { Text = "Value types store data directly, reference types store a reference to data", IsCorrect = true, Question = questions[1], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "They are exactly the same", IsCorrect = false, Question = questions[1], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Reference types are always faster", IsCorrect = false, Question = questions[1], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Value types cannot be modified after creation", IsCorrect = false, Question = questions[1], IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Question 3: Normalization
                new Choice { Text = "Process of organizing data to reduce redundancy", IsCorrect = true, Question = questions[3], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Creating database indexes", IsCorrect = false, Question = questions[3], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Backing up database data", IsCorrect = false, Question = questions[3], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Encrypting database content", IsCorrect = false, Question = questions[3], IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Question 4: SQL JOIN
                new Choice { Text = "SELECT * FROM TableA INNER JOIN TableB ON TableA.id = TableB.id", IsCorrect = true, Question = questions[4], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "SELECT * FROM TableA, TableB", IsCorrect = false, Question = questions[4], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "JOIN TableA WITH TableB", IsCorrect = false, Question = questions[4], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "MERGE TableA AND TableB", IsCorrect = false, Question = questions[4], IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Question 6: MVC Pattern
                new Choice { Text = "Model-View-Controller architectural pattern", IsCorrect = true, Question = questions[6], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Multiple Virtual Connections", IsCorrect = false, Question = questions[6], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Main Variable Controller", IsCorrect = false, Question = questions[6], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Memory View Cache", IsCorrect = false, Question = questions[6], IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Question 8: Authentication vs Authorization
                new Choice { Text = "Authentication verifies identity, authorization verifies permissions", IsCorrect = true, Question = questions[8], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "They are the same thing", IsCorrect = false, Question = questions[8], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Authorization happens before authentication", IsCorrect = false, Question = questions[8], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "Authentication is only used in web applications", IsCorrect = false, Question = questions[8], IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Question 11: Foreign key
                new Choice { Text = "A field that links to the primary key of another table", IsCorrect = true, Question = questions[11], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "A unique identifier in a table", IsCorrect = false, Question = questions[11], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "A type of index", IsCorrect = false, Question = questions[11], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new Choice { Text = "A database constraint", IsCorrect = false, Question = questions[11], IsDeleted = false, CreatedAt = DateTime.UtcNow }
            ];
        }

     /*   public static List<ExamQuestion> GetExamQuestions(List<Exam> exams, List<Question> questions)
        {
            return
            [
                // Programming Midterm Exam (Exam 0)
                new ExamQuestion { Exam = exams[0], Question = questions[0], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { Exam = exams[0], Question = questions[1], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { Exam = exams[0], Question = questions[2], IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Database Final Exam (Exam 1)
                new ExamQuestion { Exam = exams[1], Question = questions[3], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { Exam = exams[1], Question = questions[4], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { Exam = exams[1], Question = questions[5], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { Exam = exams[1], Question = questions[11], IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Web Development Quiz (Exam 2)
                new ExamQuestion { Exam = exams[2], Question = questions[6], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { Exam = exams[2], Question = questions[7], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { Exam = exams[2], Question = questions[8], IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Data Structures Midterm (Exam 3)
                new ExamQuestion { Exam = exams[3], Question = questions[9], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { Exam = exams[3], Question = questions[0], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { Exam = exams[3], Question = questions[10], IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Software Engineering Final (Exam 4)
                new ExamQuestion { Exam = exams[4], Question = questions[7], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { Exam = exams[4], Question = questions[5], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { Exam = exams[4], Question = questions[10], IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Programming Final Exam (Exam 5)
                new ExamQuestion { Exam = exams[5], Question = questions[2], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { Exam = exams[5], Question = questions[9], IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamQuestion { Exam = exams[5], Question = questions[10], IsDeleted = false, CreatedAt = DateTime.UtcNow }
            ];
        }*/

      /*  public static List<CourseEnrollment> GetCourseEnrollments(List<Student> students, List<Course> courses)
        {
            return
            [
                // Student 0 (Mohammed Ibrahim) enrollments
                new CourseEnrollment { Student = students[0], Course = courses[0], EnrollmentAt = new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { Student = students[0], Course = courses[1], EnrollmentAt = new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { Student = students[0], Course = courses[3], EnrollmentAt = new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Student 1 (Noor Abdullah) enrollments
                new CourseEnrollment { Student = students[1], Course = courses[0], EnrollmentAt = new DateTime(2024, 9, 2, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { Student = students[1], Course = courses[1], EnrollmentAt = new DateTime(2024, 9, 2, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { Student = students[1], Course = courses[2], EnrollmentAt = new DateTime(2024, 9, 2, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { Student = students[1], Course = courses[4], EnrollmentAt = new DateTime(2024, 9, 2, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Student 2 (Layla Saeed) enrollments
                new CourseEnrollment { Student = students[2], Course = courses[0], EnrollmentAt = new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { Student = students[2], Course = courses[1], EnrollmentAt = new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { Student = students[2], Course = courses[2], EnrollmentAt = new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { Student = students[2], Course = courses[4], EnrollmentAt = new DateTime(2024, 9, 3, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Student 3 (Omar Khalid) enrollments
                new CourseEnrollment { Student = students[3], Course = courses[0], EnrollmentAt = new DateTime(2024, 9, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { Student = students[3], Course = courses[2], EnrollmentAt = new DateTime(2024, 9, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new CourseEnrollment { Student = students[3], Course = courses[3], EnrollmentAt = new DateTime(2024, 9, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow }
            ];
        }*/

      /*  public static List<ExamAssignment> GetExamAssignments(List<Student> students, List<Exam> exams)
        {
            return
            [
                // Student 0 (Mohammed Ibrahim) - Based on studentExamGrades.json
                new ExamAssignment { Student = students[0], Exam = exams[0], SubmissionDate = new DateTime(2024, 12, 20, 15, 30, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamAssignment { Student = students[0], Exam = exams[1], SubmissionDate = new DateTime(2024, 12, 27, 16, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamAssignment { Student = students[0], Exam = exams[2], SubmissionDate = new DateTime(2024, 12, 21, 16, 15, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Student 1 (Noor Abdullah)
                new ExamAssignment { Student = students[1], Exam = exams[0], SubmissionDate = new DateTime(2024, 12, 20, 15, 45, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamAssignment { Student = students[1], Exam = exams[1], SubmissionDate = new DateTime(2024, 12, 27, 16, 10, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamAssignment { Student = students[1], Exam = exams[3], SubmissionDate = new DateTime(2024, 12, 22, 11, 20, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Student 2 (Layla Saeed)
                new ExamAssignment { Student = students[2], Exam = exams[0], SubmissionDate = new DateTime(2024, 12, 20, 16, 0, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamAssignment { Student = students[2], Exam = exams[1], SubmissionDate = new DateTime(2024, 12, 27, 16, 20, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamAssignment { Student = students[2], Exam = exams[4], SubmissionDate = new DateTime(2024, 12, 23, 12, 15, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },

                // Student 3 (Omar Khalid)
                new ExamAssignment { Student = students[3], Exam = exams[2], SubmissionDate = new DateTime(2024, 12, 21, 16, 30, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow },
                new ExamAssignment { Student = students[3], Exam = exams[5], SubmissionDate = new DateTime(2024, 12, 24, 10, 10, 0, DateTimeKind.Utc), IsDeleted = false, CreatedAt = DateTime.UtcNow }
            ];
        }*/

        public static async Task SeedAdminUser(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
{
    // Create admin role
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
    }

    // Create admin user
    var adminUser = new User
    {
        // Id auto-generated with Guid.CreateVersion7()
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
