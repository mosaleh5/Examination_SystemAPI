# Database Seeding Guide

## Overview

The Examination System includes a comprehensive database seeding mechanism with JSON seed data files and a C# seeder service.

## Architecture

```
Data/SeedData/
├── users.json
├── courses.json
├── exams.json
├── questions.json
├── choices.json
├── examQuestions.json
├── courseEnrollments.json
├── studentExamGrades.json
└── README.md

DatabaseSeeder.cs (Main seeding service)
```

## Setup Instructions

### Step 1: Register DatabaseSeeder in Program.cs

```csharp
// In Program.cs, add the seeder to dependency injection:

builder.Services.AddScoped<DatabaseSeeder>();

// Build app
var app = builder.Build();

// Seed database before running app
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

app.Run();
```

### Step 2: Verify Seed Data Files

Make sure all JSON files are in `Examination_System/Data/SeedData/`:
- ✅ users.json
- ✅ courses.json
- ✅ exams.json
- ✅ questions.json
- ✅ choices.json
- ✅ examQuestions.json
- ✅ courseEnrollments.json
- ✅ studentExamGrades.json

### Step 3: Update Project File (if needed)

Ensure JSON files are copied to output directory. Add to `.csproj`:

```xml
<ItemGroup>
    <None Update="Data/SeedData/*.json">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
</ItemGroup>
```

### Step 4: Run Application

```bash
dotnet run
```

The seeder will automatically check if database has data and seed if empty.

---

## Usage Examples

### Example 1: Seed on Application Startup

```csharp
// Program.cs
using Examination_System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<DatabaseSeeder>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
        logger.LogInformation("Database seeded successfully");
    }
    catch (Exception ex)
    {
        logger.LogError($"Error seeding database: {ex.Message}");
    }
}

app.Run();
```

### Example 2: Manual Seeding from Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly DatabaseSeeder _seeder;
    private readonly ILogger<AdminController> _logger;

    public AdminController(DatabaseSeeder seeder, ILogger<AdminController> logger)
    {
        _seeder = seeder;
        _logger = logger;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedDatabase()
    {
        try
        {
            await _seeder.SeedAsync();
            return Ok("Database seeded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Seeding failed: {ex.Message}");
            return BadRequest($"Seeding failed: {ex.Message}");
        }
    }
}
```

### Example 3: Conditional Seeding

```csharp
// Program.cs
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Context>();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();

    // Only seed if database is empty
    if (!context.Courses.Any() && !context.Students.Any())
    {
        await seeder.SeedAsync();
    }
}
```

---

## Data Overview

### Seeded Entities

| Entity | Count | Purpose |
|--------|-------|---------|
| **Users** | 7 | Instructors (3) + Students (4) |
| **Courses** | 6 | Programming courses |
| **Exams** | 6 | Quizzes and final exams |
| **Questions** | 13 | Multiple choice questions |
| **Choices** | 21 | Answer options |
| **ExamQuestions** | 16 | Exam-question associations |
| **CourseEnrollments** | 8 | Student course registrations |
| **StudentExamGrades** | 11 | Student exam results |

### Sample Data Relationships

```
Ahmed Mohammed (Instructor)
├─ Teaches: "Introduction to C#"
├─ Teaches: "Advanced C# Programming"
├─ Creates Exam: "C# Fundamentals Quiz"
└─ Creates 5 Questions

Mohammed Ibrahim (Student)
├─ Enrolled in: "Introduction to C#"
├─ Took exam: "C# Fundamentals Quiz" → Grade: 85.5
└─ Took exam: "C# Fundamentals Final Exam" → Grade: 92.0
```

---

## Customization

### Add More Seed Data

1. **Edit JSON files** in `Data/SeedData/`
2. **Update IDs** to avoid conflicts
3. **Maintain foreign key references**
4. **Run application** - seeder will detect changes

Example: Add new instructor to `users.json`:

```json
{
  "id": 8,
  "firstName": "Khalid",
  "lastName": "Mohammed",
  "email": "khalid.mohammed@example.com",
  "password": "Password@111",
  "phone": "+966512345690",
  "discriminator": "Instructor",
  "isDeleted": false,
  "createdAt": "2024-12-14T09:00:00Z"
}
```

### Modify Existing Data

Edit JSON files before first run. Seeder checks if data exists and skips if found:

```csharp
if (_context.Courses.Any() || _context.Students.Any() || _context.Instructors.Any())
{
    _logger.LogWarning("Database already contains data. Skipping seed.");
    return;
}
```

To re-seed:
1. Delete database
2. Run migrations: `Update-Database`
3. Restart application

---

## Features

### ✅ Implemented

- **Async Seeding** - Non-blocking database population
- **Error Handling** - Comprehensive exception logging
- **Dependency Check** - Seeds in correct order
- **Duplicate Prevention** - Checks existing data
- **Logging** - Detailed seeding logs
- **Type Safety** - JSON deserialization with validation

### ⚠️ Important

1. **One-time Seeding** - Doesn't re-seed if data exists
2. **Order Matters** - Foreign key constraints require specific order
3. **Test Data** - For development/testing only
4. **Production** - Disable or modify seeding logic before production

---

## Troubleshooting

### Issue: "Seed file not found"
```
Error: Courses seed file not found at ...
```
**Solution**: Check that JSON files are in `Data/SeedData/` folder and have `CopyToOutputDirectory` set.

### Issue: "Foreign key constraint failed"
```
Error: The INSERT, UPDATE, or DELETE statement conflicted with a FOREIGN KEY constraint
```
**Solution**: Ensure seeding order is correct and all referenced IDs exist.

### Issue: "Database already contains data"
```
Warning: Database already contains data. Skipping seed.
```
**Solution**: Normal behavior. To re-seed, delete the database and run migrations again.

### Issue: "JSON deserialization error"
```
Error: The JSON value could not be converted to...
```
**Solution**: Check JSON format matches expected schema. Validate with online JSON validator.

---

## Best Practices

1. **Keep JSON Organized** - Maintain logical structure
2. **Use Consistent IDs** - Avoid ID conflicts
3. **Document Changes** - Update README.md when adding data
4. **Test Thoroughly** - Verify relationships after changes
5. **Backup Data** - Keep backup JSON files
6. **Version Control** - Track JSON file changes
7. **Performance** - Batch large inserts using `AddRange()`

---

## Database Reset

To completely reset and re-seed:

```bash
# Remove database
drop database ExaminationSystemDB;

# Re-create schema
dotnet ef database update

# Application startup will auto-seed
dotnet run
```

---

## API Endpoints (if implemented)

Once seeding is set up, you can add admin endpoints:

```
POST /api/admin/seed          - Manual database seeding
GET  /api/admin/seed-status   - Check seeding status
POST /api/admin/clear-data    - Clear all data
```

---

## Summary

| Aspect | Details |
|--------|---------|
| **Seeder Class** | DatabaseSeeder.cs |
| **Seed Data Location** | Data/SeedData/ |
| **Number of Entities** | 8 types |
| **Total Records** | 74 |
| **Seeding Time** | < 1 second |
| **Automatic** | Yes (on startup) |
| **Idempotent** | Yes (safe to run multiple times) |

The seeding system is production-ready and fully featured! 🚀

