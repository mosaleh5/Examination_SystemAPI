# How to Apply the Fixed Migration

## Prerequisites

Make sure you have the correct connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ExaminationSystemDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

## Option 1: Using Package Manager Console (Visual Studio)

### Step 1: Open Package Manager Console
- In Visual Studio: **Tools** → **NuGet Package Manager** → **Package Manager Console**

### Step 2: Make sure default project is set
- Select `Examination_System` in the dropdown at the top of Package Manager Console

### Step 3: Run the migration
```powershell
Update-Database
```

Or specify the migration explicitly:
```powershell
Update-Database -Migration AddCompleteSchema
```

### Step 4: Verify
```powershell
# List all applied migrations
Get-Migrations

# Check current database version
Update-Database -Script
```

---

## Option 2: Using .NET CLI (Command Line)

### Step 1: Navigate to project directory
```bash
cd D:\Courses\Projects\Examination_System\Examination_System
```

### Step 2: Run the migration
```bash
dotnet ef database update
```

Or specify the migration:
```bash
dotnet ef database update AddCompleteSchema
```

### Step 3: Verify
```bash
# List all migrations
dotnet ef migrations list

# Get the SQL script (without applying)
dotnet ef database update --script
```

---

## Option 3: Manual SQL Script

### Generate the SQL script
```powershell
# In Package Manager Console
Update-Database -Script -SourceMigration 0
```

Or with dotnet CLI:
```bash
dotnet ef migrations script --idempotent > migration.sql
```

Then run `migration.sql` against your database manually in SQL Server Management Studio.

---

## Troubleshooting

### Issue 1: "Database does not exist"
**Solution**: The migration will create the database if it doesn't exist. Just run `Update-Database`.

### Issue 2: "Table already exists"
**Solution**: This migration drops and recreates tables. If you have existing data, back it up first:
```sql
-- Backup existing data before migration
BACKUP DATABASE ExaminationSystemDB 
TO DISK = 'C:\Backups\ExaminationSystemDB_backup.bak'
```

### Issue 3: "Cannot drop table, foreign key constraint exists"
**Solution**: The migration handles this automatically. If you get this error, run:
```powershell
# Revert to initial migration
Update-Database -Migration init

# Then apply the new migration
Update-Database
```

### Issue 4: "The column name is not valid"
**Solution**: Clean rebuild and try again:
```bash
dotnet clean
dotnet build
dotnet ef database update
```

---

## Verifying the Migration Worked

### Check database schema
```sql
-- List all tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' 
ORDER BY TABLE_NAME

-- Should see: Choices, Courses, CourseEnrollments, Exams, ExamQuestions, 
--             Questions, StudentExamGrades, Users
```

### Check User table with discriminator
```sql
SELECT * FROM Users
-- Should have columns: Id, FirstName, LastName, Email, Password, Phone, 
--                      Discriminator, Major, EnrollmentDate
```

### Check foreign key relationships
```sql
SELECT 
    CONSTRAINT_NAME,
    TABLE_NAME,
    COLUMN_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME, CONSTRAINT_NAME
```

### Check indexes
```sql
SELECT 
    TABLE_NAME,
    INDEX_NAME,
    COLUMN_NAME
FROM INFORMATION_SCHEMA.STATISTICS
WHERE TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME, INDEX_NAME
```

---

## Rolling Back (If Needed)

If something goes wrong, you can roll back to the previous migration:

```powershell
# List migrations
Get-Migrations

# Rollback to init migration (or any previous migration)
Update-Database -Migration init

# Then reapply
Update-Database -Migration AddCompleteSchema
```

---

## Common Migration Commands

```powershell
# List all migrations
Get-Migrations

# Get current database migration
Update-Database -Script

# Remove last migration (if not applied)
Remove-Migration

# Create new migration (only if you change models)
Add-Migration MigrationName

# See SQL that will be executed (without applying)
Update-Database -Script -SourceMigration 0

# Apply specific migration
Update-Database -Migration AddCompleteSchema

# Rollback all migrations
Update-Database -Migration 0
```

---

## After Migration Success

1. ✅ Verify database tables were created
2. ✅ Verify all foreign keys are in place
3. ✅ Verify no duplicate columns exist
4. ✅ Test creating records (Exam, Course, Student, etc.)
5. ✅ Test validation rules work as expected

---

## If You Need to Modify Models Later

If you modify any entity models (add/remove properties), you'll need to create a new migration:

```powershell
# Create migration
Add-Migration DescriptiveNameForChanges

# Review the generated migration file
# Edit if needed

# Apply
Update-Database
```

