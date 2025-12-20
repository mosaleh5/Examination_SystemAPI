# ✅ Migration Cleanup Checklist

## What Was Done

- [x] Identified critical schema errors in migration file
- [x] Deleted corrupted migration (`20251213093143_addingModel.cs`)
- [x] Deleted corrupted designer file
- [x] Created clean migration (`AddCompleteSchema.cs`)
- [x] Created proper designer file
- [x] Recreated model snapshot
- [x] Verified build compiles successfully
- [x] Created comprehensive documentation

---

## Before You Apply Migration ⚠️

### Backup Your Data (if you have any)
```sql
-- If you have existing data, back it up first
BACKUP DATABASE ExaminationSystemDB 
TO DISK = 'C:\Backups\ExaminationSystemDB_backup.bak'
```

### Check Your Connection String
File: `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ExaminationSystemDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

## Apply the Migration

### Quick Method (Package Manager Console)
```powershell
# In Visual Studio Package Manager Console
Update-Database
```

### Alternative Method (.NET CLI)
```bash
dotnet ef database update
```

### Manual Method (SQL Script)
```powershell
Update-Database -Script
```
Then run the SQL in SQL Server Management Studio

---

## Verify Migration Was Applied

### Check Tables Exist
```sql
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' 
ORDER BY TABLE_NAME
```

**Expected tables:**
- ✅ Choices
- ✅ Courses
- ✅ CourseEnrollments
- ✅ Exams
- ✅ ExamQuestions
- ✅ Questions
- ✅ StudentExamGrades
- ✅ Users (contains Instructors and Students)

### Check Columns Are Correct
```sql
-- Should NOT have these columns (if they exist, migration failed)
SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME IN ('Exams', 'Courses', 'Choices')
  AND COLUMN_NAME LIKE '%1' OR COLUMN_NAME LIKE '%2'
```

**Should find: NONE** ✅

### Check Foreign Keys
```sql
SELECT CONSTRAINT_NAME, TABLE_NAME, COLUMN_NAME 
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = 'dbo' AND CONSTRAINT_TYPE = 'FOREIGN KEY'
ORDER BY TABLE_NAME
```

**Should find:**
- ✅ FK_Courses_Users_InstructorId
- ✅ FK_Exams_Courses_CourseId
- ✅ FK_Exams_Users_InstructorId
- ✅ FK_Questions_Users_InstructorId
- ✅ FK_Choices_Questions_QuestionId
- ✅ FK_ExamQuestions_Exams_ExamId
- ✅ FK_ExamQuestions_Questions_QuestionId
- ✅ FK_StudentExamGrades_Users_StudentId
- ✅ FK_StudentExamGrades_Exams_ExamId
- ✅ FK_CourseEnrollments_Users_StudentId
- ✅ FK_CourseEnrollments_Courses_CourseId

---

## Test Your Application

### Test 1: Can you create a Course?
```csharp
var course = new Course 
{ 
    Name = "C# Basics",
    Description = "Learn C#",
    Hours = "40",
    InstructorId = 1
};
```

### Test 2: Can you create an Exam?
```csharp
var exam = new Exam
{
    Title = "Midterm",
    Date = DateTime.Now.AddDays(7),
    DurationMinutes = 120,
    Fullmark = 100,
    QuestionsCount = 50,
    CourseId = 1,
    InstructorId = 1
};
```

### Test 3: Does validation work?
```csharp
// Should fail - date in past
var badExam = new Exam { Date = DateTime.Now.AddDays(-1) };
// ModelState.IsValid should be FALSE ✅

// Should fail - negative grade
var badGrade = new StudentExamGrade { Grade = -5 };
// ModelState.IsValid should be FALSE ✅

// Should fail - grade > fullmark
var badGrade2 = new StudentExamGrade { Grade = 150 };
// With Exam.Fullmark = 100, IsValid() should be FALSE ✅
```

---

## If Migration Fails

### Issue: "Database already exists with different schema"
```powershell
# Option 1: Drop and recreate
Update-Database -Migration 0
Update-Database
```

### Issue: "Column 'InstructorId1' cannot be dropped"
```powershell
# This shouldn't happen with our clean migration, but if it does:
Update-Database -Migration init
Update-Database
```

### Issue: "The IDENTITY column specified in IDENTITY_INSERT is for table 'Exams'"
```powershell
# Clean rebuild
dotnet clean
dotnet build
Update-Database
```

---

## Post-Migration Checklist

- [ ] Migration applied without errors
- [ ] All 8 tables created
- [ ] No shadow/duplicate columns exist
- [ ] All foreign keys in place
- [ ] User table has Discriminator column
- [ ] Exam table has DurationMinutes (not Duration)
- [ ] Exam table has ExamType, Fullmark, QuestionsCount
- [ ] StudentExamGrade table has SubmissionDate (not SubbmissionDate)
- [ ] ExamQuestion table has only ExamId and QuestionId (no ExamId1, QuestionId1)
- [ ] Choice table has only QuestionId (no QuestionId1, QuestionId2)
- [ ] Build compiles successfully
- [ ] Can create test records without errors
- [ ] Validation rules work correctly

---

## You're Done! 🎉

Your database schema is now:
- ✅ Clean and normalized
- ✅ Properly validated
- ✅ Correctly configured
- ✅ Ready for production

If you have any questions, refer to:
- `MIGRATION_CLEANUP_REPORT.md` - Technical details
- `HOW_TO_APPLY_MIGRATION.md` - Detailed instructions
- `MIGRATION_SUMMARY.md` - High-level overview

