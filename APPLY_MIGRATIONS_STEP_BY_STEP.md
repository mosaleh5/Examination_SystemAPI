# 🚀 APPLY MIGRATIONS - STEP BY STEP

## Prerequisites
✅ Build compiles successfully
✅ Two clean migrations ready
✅ Visual Studio or .NET CLI installed

---

## Method 1: Package Manager Console (Visual Studio) - RECOMMENDED

### Step 1: Open Package Manager Console
1. In Visual Studio: **Tools** → **NuGet Package Manager** → **Package Manager Console**
2. Make sure "Examination_System" is selected in the dropdown at top

### Step 2: Apply Cleanup Migration
Copy and paste this command:
```powershell
Update-Database -Migration CleanupOldTables
```

**Expected output:**
```
Applying migration '20251213171320_CleanupOldTables'.
Done.
```

### Step 3: Verify Cleanup (Optional)
Open SQL Server Management Studio and run:
```sql
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo'
```

**Expected result:** No rows returned (all tables dropped)

### Step 4: Apply Init Migration
```powershell
Update-Database
```

**Expected output:**
```
Applying migration '20251213171326_init'.
Done.
```

### Step 5: Verify Final Schema (Optional)
```sql
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' 
ORDER BY TABLE_NAME
```

**Expected result:**
```
Choices
CourseEnrollments
Courses
ExamQuestions
Exams
Instructors
Questions
StudentExamGrades
Students
```

✅ **Database is ready!**

---

## Method 2: .NET CLI (Command Line)

### Step 1: Open PowerShell/CMD
Navigate to project directory:
```bash
cd D:\Courses\Projects\Examination_System\Examination_System
```

### Step 2: Apply Cleanup Migration
```bash
dotnet ef database update --migration CleanupOldTables
```

### Step 3: Apply Init Migration
```bash
dotnet ef database update
```

### Step 4: Verify
```bash
dotnet ef migrations list
```

---

## Troubleshooting

### Issue: "Migration not found"
**Solution:** Make sure migrations are in: `Examination_System/Migrations/`
- ✅ 20251213171320_CleanupOldTables.cs
- ✅ 20251213171320_CleanupOldTables.Designer.cs
- ✅ 20251213171326_init.cs
- ✅ 20251213171326_init.Designer.cs

### Issue: "Database does not exist"
**Solution:** Migrations will create it automatically. Just run the command again.

### Issue: "Foreign key constraint failed"
**Solution:** Make sure you applied the CleanupOldTables migration FIRST.

### Issue: "Table already exists"
**Solution:** This shouldn't happen if you followed the steps. But if it does:
```powershell
# Revert to no migrations
Update-Database -Migration 0

# Then apply from scratch
Update-Database
```

---

## Rollback (If Needed)

If something goes wrong, revert all migrations:
```powershell
Update-Database -Migration 0
```

Then check what went wrong and try again.

---

## Summary

✅ **Total time:** < 5 minutes  
✅ **Two migrations needed:** CleanupOldTables → init  
✅ **Result:** Clean database with 9 tables and proper relationships  

**That's it! You're done!** 🎉

