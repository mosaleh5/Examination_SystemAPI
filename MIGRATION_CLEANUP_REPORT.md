# Migration Cleanup & Repair Report

## Problem Identified

The previous migration file `20251213093143_addingModel.cs` contained **critical schema errors** that would have caused:

1. **Duplicate Shadow Columns** - Unnecessary columns like `InstructorId1`, `CourseId1`, `QuestionId1`, `QuestionId2`, `ExamId1`
2. **Broken Foreign Key Relationships** - Multiple redundant foreign keys pointing to the same entities
3. **Data Integrity Issues** - Confused relationships that could lead to data corruption
4. **Model Mismatch** - Schema didn't align with current model definitions

### Examples of Issues:

```sql
-- ❌ Bad: Duplicate columns created
ALTER TABLE Exams ADD CourseId1 INT NULL;  -- Already have CourseId
ALTER TABLE Exams ADD InstructorId1 INT;   -- Already have InstructorId
ALTER TABLE Choices ADD QuestionId1 INT;   -- Already have QuestionId
ALTER TABLE Choices ADD QuestionId2 INT;   -- Unnecessary duplicate

-- ❌ Bad: Multiple foreign keys for same relationship
FK_Exams_Courses_CourseId (CourseId)
FK_Exams_Courses_CourseId1 (CourseId1)  -- Redundant!

FK_Questions_Instructors_InstructorId
FK_Questions_Instructors_InstructorId1  -- Redundant!
```

---

## Solution Implemented

### Step 1: Removed Corrupted Migration Files
- ❌ Deleted `20251213093143_addingModel.cs`
- ❌ Deleted `20251213093143_addingModel.Designer.cs`

### Step 2: Created Clean Migration
- ✅ Created new `20251213093143_AddCompleteSchema.cs` with proper schema
- ✅ Created matching `20251213093143_AddCompleteSchema.Designer.cs`
- ✅ Recreated `ContextModelSnapshot.cs` with correct model state

### Step 3: Migration Structure

The new migration follows EF Core best practices:

#### **Phase 1: Prepare tables**
- Drop old foreign keys
- Drop old primary keys
- Rename tables to proper casing

#### **Phase 2: Create User/Auth tables**
```sql
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(MAX) NOT NULL,
    LastName NVARCHAR(MAX) NOT NULL,
    Email NVARCHAR(MAX) NOT NULL,
    Password NVARCHAR(MAX) NOT NULL,
    Phone NVARCHAR(MAX) NOT NULL,
    Discriminator NVARCHAR(8) NOT NULL,  -- TPH: "User", "Instructor", "Student"
    Major NVARCHAR(MAX) NULL,
    EnrollmentDate DATETIME2 NULL
)
```

#### **Phase 3: Update existing tables**
- Rename `courses` → `Courses`
- Rename `exams` → `Exams`
- Add missing columns (InstructorId, etc.)
- Update foreign keys

#### **Phase 4: Create new tables**
- `Questions` - Connected to Instructors
- `Choices` - Question answer options
- `ExamQuestions` - Many-to-many: Exams ↔ Questions
- `StudentExamGrades` - Student exam results
- `CourseEnrollments` - Student enrollments

---

## Database Schema (Corrected)

### Entity Relationships

```
User (Base Class)
├─ Instructor (via TPH Discriminator)
│  ├─ Courses (1:M) [FK: InstructorId]
│  ├─ Exams (1:M) [FK: InstructorId]
│  └─ Questions (1:M) [FK: InstructorId]
│
└─ Student (via TPH Discriminator)
   ├─ Exams [StudentExamGrades] (M:M)
   └─ Courses [CourseEnrollments] (M:M)

Course
├─ Exams (1:M) [FK: CourseId]
├─ CourseEnrollments (1:M) [FK: CourseId]
└─ Instructor (M:1) [FK: InstructorId]

Exam
├─ StudentExamGrades (1:M) [FK: ExamId]
├─ ExamQuestions (1:M) [FK: ExamId]
├─ Course (M:1) [FK: CourseId]
└─ Instructor (M:1) [FK: InstructorId]

Question
├─ Choices (1:M) [FK: QuestionId]
├─ ExamQuestions (1:M) [FK: QuestionId]
└─ Instructor (M:1) [FK: InstructorId]

StudentExamGrade
├─ Student (M:1) [FK: StudentId]
└─ Exam (M:1) [FK: ExamId]

CourseEnrollment
├─ Student (M:1) [FK: StudentId]
└─ Course (M:1) [FK: CourseId]
```

---

## Table of Entities & Columns

| Table | PK | Foreign Keys | Columns |
|-------|----|----|---------|
| **Users** | Id | - | Id, FirstName, LastName, Email, Password, Phone, Discriminator, Major*, EnrollmentDate* |
| **Courses** | CourseId | InstructorId | CourseId, Name, Description, Hours, InstructorId |
| **Exams** | Id | CourseId, InstructorId | Id, Title, Date, DurationMinutes, Fullmark, ExamType, QuestionsCount, CourseId, InstructorId |
| **Questions** | Id | InstructorId | Id, Title, mark, Level, InstructorId |
| **Choices** | Id | QuestionId | Id, Text, IsCorrect, QuestionId |
| **ExamQuestions** | Id | ExamId, QuestionId | Id, ExamId, QuestionId |
| **StudentExamGrades** | Id | StudentId, ExamId | Id, StudentId, ExamId, Grade, SubmissionDate |
| **CourseEnrollments** | Id | StudentId, CourseId | Id, StudentId, CourseId, EnrollmentAt |

*= Nullable for base User class

---

## Delete Behavior Configuration

| Relationship | Delete Behavior | Rationale |
|--------------|-----------------|-----------|
| Instructor → Courses | **RESTRICT** | Don't delete courses when instructor deleted |
| Instructor → Exams | **RESTRICT** | Don't delete exams when instructor deleted |
| Instructor → Questions | **RESTRICT** | Don't delete questions when instructor deleted |
| Course → Exams | **RESTRICT** | Don't delete exams when course deleted |
| Course → CourseEnrollments | **RESTRICT** | Don't delete enrollments when course deleted |
| Exam → ExamQuestions | **CASCADE** | Delete exam questions when exam deleted |
| Exam → StudentExamGrades | **CASCADE** | Delete grades when exam deleted |
| Question → Choices | **CASCADE** | Delete choices when question deleted |
| Question → ExamQuestions | **RESTRICT** | Keep exam questions when question deleted |
| Student → CourseEnrollments | **CASCADE** | Delete enrollments when student deleted |
| Student → StudentExamGrades | **CASCADE** | Delete grades when student deleted |

---

## Validation Features Applied

All entities include proper validation (from previous implementation):

✅ **Exam Validation**
- Date must be in future
- DurationMinutes must be > 0
- Fullmark must be > 0
- QuestionsCount must be > 0

✅ **Grade Validation**
- Grade must be ≥ 0
- Grade must be ≤ Exam.Fullmark

---

## Migration Verification

✅ **Build Status**: Successful
✅ **Schema**: Clean and normalized
✅ **Foreign Keys**: Properly configured
✅ **Discriminator**: Configured for TPH inheritance
✅ **Indexes**: Created for all foreign keys

---

## Next Steps

To apply this migration to your database:

```bash
# In Package Manager Console:
Update-Database -Migration AddCompleteSchema

# Or using dotnet CLI:
dotnet ef database update --migration AddCompleteSchema
```

### To verify the migration was applied:

```sql
-- Check tables created
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo'

-- Check User table with discriminator
SELECT * FROM Users

-- Check foreign key relationships
SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
```

---

## Summary of Changes

| Item | Before | After |
|------|--------|-------|
| Duplicate columns | ❌ 5+ shadow columns | ✅ None |
| Foreign key relationships | ❌ Broken/redundant | ✅ Clean & proper |
| Table naming | ❌ Mixed case (courses, exams) | ✅ PascalCase |
| Schema alignment | ❌ Mismatched models | ✅ Matches all entities |
| Data integrity | ❌ At risk | ✅ Protected |

---

## Files Modified

| File | Action | Purpose |
|------|--------|---------|
| `20251213093143_addingModel.cs` | 🗑️ DELETED | Removed corrupted migration |
| `20251213093143_addingModel.Designer.cs` | 🗑️ DELETED | Removed corrupted designer |
| `20251213093143_AddCompleteSchema.cs` | ✨ CREATED | Clean migration with correct schema |
| `20251213093143_AddCompleteSchema.Designer.cs` | ✨ CREATED | Proper designer file |
| `ContextModelSnapshot.cs` | ♻️ RECREATED | Accurate model state |

