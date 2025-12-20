# Database Schema Diagram

## Entity Relationship Diagram (ERD)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          USERS (Base Table - TPH)                            │
├─────────────────────────────────────────────────────────────────────────────┤
│ PK: Id (INT)                                                                  │
│ FirstName, LastName, Email, Password, Phone (NVARCHAR)                       │
│ Discriminator (NVARCHAR(8)) → "User", "Instructor", or "Student"             │
│ Major (NVARCHAR) - NULL unless Discriminator = "Student"                      │
│ EnrollmentDate (DATETIME2) - NULL unless Discriminator = "Student"            │
└─────────────────────────────────────────────────────────────────────────────┘
         ▲                              ▲
         │                              │
         │ Discriminator               │ Discriminator
         │ = "Instructor"              │ = "Student"
         │                              │
    ┌────────────────────┐         ┌──────────────────────┐
    │   INSTRUCTOR       │         │     STUDENT          │
    │   (View/TPH)       │         │    (View/TPH)        │
    └────────────────────┘         └──────────────────────┘
         │                              │
         │ Has Many (1:M)              │ Many-to-Many
         ▼                              ▼
    ┌──────────────────────────────────────────┐
    │              COURSES                      │
    ├──────────────────────────────────────────┤
    │ PK: CourseId                              │
    │ Name, Description, Hours                  │
    │ FK: InstructorId ──► USERS.Id             │
    └──────────────────────────────────────────┘
         │ Has Many (1:M)
         │
         ├─────────────────────────────┬─────────────────────────┐
         │                             │                         │
         ▼                             ▼                         ▼
    ┌──────────────┐        ┌──────────────────────┐    ┌─────────────────────┐
    │    EXAMS     │        │ COURSEENROLLMENTS    │    │  (join table)       │
    ├──────────────┤        ├──────────────────────┤    │                     │
    │ PK: Id       │        │ PK: Id               │    │ Represents M:M      │
    │ Title, Date  │        │ FK: StudentId ──┐    │    │ Course ──► Student  │
    │ DurationMins │        │ FK: CourseId ───┼──► │    └─────────────────────┘
    │ Fullmark     │        │ EnrollmentAt     │    │
    │ ExamType     │        └──────────────────────┘
    │ Questions... │             ▲
    │ FK: CourseId │──┐          │
    │ FK: Instructor │─┼─────────┘
    │    Id       │  │
    └──────────────┘  │
         │            │
         │ Has Many  │
         │           │
         ├───────────┴──────────────────┐
         │                              │
         ▼                              ▼
    ┌──────────────────────┐    ┌──────────────────────┐
    │  EXAMQUESTIONS       │    │ STUDENTEXAMGRADES    │
    │  (Many-to-Many)      │    ├──────────────────────┤
    ├──────────────────────┤    │ PK: Id               │
    │ PK: Id               │    │ FK: StudentId        │
    │ FK: ExamId           │    │ FK: ExamId           │
    │ FK: QuestionId       │    │ Grade (FLOAT)        │
    └──────────────────────┘    │ SubmissionDate       │
         │                       └──────────────────────┘
         │
         ▼
    ┌──────────────────────┐
    │    QUESTIONS         │
    ├──────────────────────┤
    │ PK: Id               │
    │ Title, Mark, Level   │
    │ FK: InstructorId ────┴──► USERS.Id
    └──────────────────────┘
         │
         │ Has Many (1:M)
         │
         ▼
    ┌──────────────────────┐
    │     CHOICES          │
    ├──────────────────────┤
    │ PK: Id               │
    │ Text, IsCorrect      │
    │ FK: QuestionId ──────┴──► QUESTIONS.Id
    └──────────────────────┘
```

---

## Table Structure Details

### USERS Table
```sql
CREATE TABLE [Users] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [FirstName] NVARCHAR(MAX) NOT NULL,
    [LastName] NVARCHAR(MAX) NOT NULL,
    [Email] NVARCHAR(MAX) NOT NULL,
    [Password] NVARCHAR(MAX) NOT NULL,
    [Phone] NVARCHAR(MAX) NOT NULL,
    [Discriminator] NVARCHAR(8) NOT NULL,          -- "User", "Instructor", "Student"
    [Major] NVARCHAR(MAX) NULL,                     -- Student only
    [EnrollmentDate] DATETIME2 NULL                 -- Student only
);

-- Indexes
CREATE INDEX [IX_Users_Discriminator] ON [Users]([Discriminator]);
```

### COURSES Table
```sql
CREATE TABLE [Courses] (
    [CourseId] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(MAX) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [Hours] NVARCHAR(MAX) NOT NULL,
    [InstructorId] INT NOT NULL,
    CONSTRAINT [FK_Courses_Users_InstructorId] 
        FOREIGN KEY ([InstructorId]) 
        REFERENCES [Users]([Id]) 
        ON DELETE RESTRICT
);

-- Indexes
CREATE INDEX [IX_Courses_InstructorId] ON [Courses]([InstructorId]);
```

### EXAMS Table
```sql
CREATE TABLE [Exams] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Title] NVARCHAR(150) NOT NULL,
    [Date] DATETIME2 NOT NULL,
    [DurationMinutes] INT NOT NULL,
    [Fullmark] INT NOT NULL,
    [ExamType] INT NOT NULL,                        -- 0=Quiz, 1=Final
    [QuestionsCount] INT NOT NULL,
    [CourseId] INT NOT NULL,
    [InstructorId] INT NOT NULL,
    CONSTRAINT [FK_Exams_Courses_CourseId] 
        FOREIGN KEY ([CourseId]) 
        REFERENCES [Courses]([CourseId]) 
        ON DELETE RESTRICT,
    CONSTRAINT [FK_Exams_Users_InstructorId] 
        FOREIGN KEY ([InstructorId]) 
        REFERENCES [Users]([Id]) 
        ON DELETE RESTRICT
);

-- Indexes
CREATE INDEX [IX_Exams_CourseId] ON [Exams]([CourseId]);
CREATE INDEX [IX_Exams_InstructorId] ON [Exams]([InstructorId]);
```

### QUESTIONS Table
```sql
CREATE TABLE [Questions] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Title] NVARCHAR(500) NOT NULL,
    [mark] INT NOT NULL,
    [Level] INT NOT NULL,                           -- 0=Simple, 1=Medium, 2=Hard
    [InstructorId] INT NOT NULL,
    CONSTRAINT [FK_Questions_Users_InstructorId] 
        FOREIGN KEY ([InstructorId]) 
        REFERENCES [Users]([Id]) 
        ON DELETE RESTRICT
);

-- Indexes
CREATE INDEX [IX_Questions_InstructorId] ON [Questions]([InstructorId]);
```

### CHOICES Table
```sql
CREATE TABLE [Choices] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Text] NVARCHAR(MAX) NOT NULL,
    [IsCorrect] BIT NOT NULL,
    [QuestionId] INT NOT NULL,
    CONSTRAINT [FK_Choices_Questions_QuestionId] 
        FOREIGN KEY ([QuestionId]) 
        REFERENCES [Questions]([Id]) 
        ON DELETE CASCADE
);

-- Indexes
CREATE INDEX [IX_Choices_QuestionId] ON [Choices]([QuestionId]);
```

### EXAMQUESTIONS Table (Many-to-Many Join)
```sql
CREATE TABLE [ExamQuestions] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [ExamId] INT NOT NULL,
    [QuestionId] INT NOT NULL,
    CONSTRAINT [FK_ExamQuestions_Exams_ExamId] 
        FOREIGN KEY ([ExamId]) 
        REFERENCES [Exams]([Id]) 
        ON DELETE CASCADE,
    CONSTRAINT [FK_ExamQuestions_Questions_QuestionId] 
        FOREIGN KEY ([QuestionId]) 
        REFERENCES [Questions]([Id]) 
        ON DELETE RESTRICT
);

-- Indexes
CREATE INDEX [IX_ExamQuestions_ExamId] ON [ExamQuestions]([ExamId]);
CREATE INDEX [IX_ExamQuestions_QuestionId] ON [ExamQuestions]([QuestionId]);
```

### STUDENTEXAMGRADES Table
```sql
CREATE TABLE [StudentExamGrades] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [StudentId] INT NOT NULL,
    [ExamId] INT NOT NULL,
    [Grade] FLOAT NOT NULL,
    [SubmissionDate] DATETIME2 NOT NULL,
    CONSTRAINT [FK_StudentExamGrades_Users_StudentId] 
        FOREIGN KEY ([StudentId]) 
        REFERENCES [Users]([Id]) 
        ON DELETE CASCADE,
    CONSTRAINT [FK_StudentExamGrades_Exams_ExamId] 
        FOREIGN KEY ([ExamId]) 
        REFERENCES [Exams]([Id]) 
        ON DELETE CASCADE
);

-- Indexes
CREATE INDEX [IX_StudentExamGrades_StudentId] ON [StudentExamGrades]([StudentId]);
CREATE INDEX [IX_StudentExamGrades_ExamId] ON [StudentExamGrades]([ExamId]);
```

### COURSEENROLLMENTS Table (Many-to-Many Join)
```sql
CREATE TABLE [CourseEnrollments] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [StudentId] INT NOT NULL,
    [CourseId] INT NOT NULL,
    [EnrollmentAt] DATETIME2 NOT NULL,
    CONSTRAINT [FK_CourseEnrollments_Users_StudentId] 
        FOREIGN KEY ([StudentId]) 
        REFERENCES [Users]([Id]) 
        ON DELETE CASCADE,
    CONSTRAINT [FK_CourseEnrollments_Courses_CourseId] 
        FOREIGN KEY ([CourseId]) 
        REFERENCES [Courses]([CourseId]) 
        ON DELETE RESTRICT
);

-- Indexes
CREATE INDEX [IX_CourseEnrollments_StudentId] ON [CourseEnrollments]([StudentId]);
CREATE INDEX [IX_CourseEnrollments_CourseId] ON [CourseEnrollments]([CourseId]);
```

---

## Data Flow Example

### Creating an Exam with Questions
```
1. Create Instructor (Users table, Discriminator="Instructor")
2. Create Course (Courses table, FK to Instructor)
3. Create Questions (Questions table, FK to Instructor)
4. Create Exam (Exams table, FK to Course & Instructor)
5. Link Questions to Exam (ExamQuestions table)
6. Create Student (Users table, Discriminator="Student")
7. Enroll Student in Course (CourseEnrollments table)
8. Student takes Exam → Create StudentExamGrade
```

---

## Key Features

✅ **Single Users Table** - TPH (Table Per Hierarchy) for both User types
✅ **No Shadow Columns** - Only essential columns
✅ **Proper Indexes** - On all foreign keys
✅ **Cascade Deletes** - For exam questions, grades, enrollments
✅ **Restrict Deletes** - For critical relationships
✅ **Many-to-Many** - Proper join tables (ExamQuestions, CourseEnrollments)
✅ **Data Validation** - Enforced at entity level

