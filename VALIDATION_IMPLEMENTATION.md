# Data Validation Implementation Guide

## Overview
This document describes the comprehensive validation system implemented to ensure data integrity for the Examination System.

---

## Issue 1: Exam Dates Cannot Be In The Past

### Solution Implemented:

#### 1. **Custom Validation Attribute** (`Exam.cs`)
```csharp
[FutureDate(ErrorMessage = "Exam date must be in the future.")]
public DateTime Date { get; set; }
```

- A custom `FutureDateAttribute` validates that exam dates are always in the future
- Applied at the model level for automatic validation

#### 2. **Usage in Controllers**
```csharp
if (!ModelState.IsValid)
{
    return BadRequest(ModelState);
}
```

---

## Issue 2: Duration Minutes Must Be Positive

### Solution Implemented:

#### 1. **Range Validation** (`Exam.cs`)
```csharp
[Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
public int DurationMinutes { get; set; }
```

#### 2. **Database Constraint** (`ExamConfigurations.cs`)
```csharp
builder.Property(e => e.DurationMinutes)
    .HasDefaultValue(0)
    .IsRequired();
```

---

## Issue 3: Full Marks Must Be Positive

### Solution Implemented:

#### 1. **Range Validation** (`Exam.cs`)
```csharp
[Range(1, int.MaxValue, ErrorMessage = "Full marks must be greater than 0.")]
public int Fullmark { get; set; }
```

#### 2. **Database Constraint** (`ExamConfigurations.cs`)
```csharp
builder.Property(e => e.Fullmark)
    .HasDefaultValue(0)
    .IsRequired();
```

---

## Issue 4: Questions Count Must Be Positive

### Solution Implemented:

#### 1. **Range Validation** (`Exam.cs`)
```csharp
[Range(1, int.MaxValue, ErrorMessage = "Questions count must be at least 1.")]
public int QuestionsCount { get; set; }
```

#### 2. **Database Constraint** (`ExamConfigurations.cs`)
```csharp
builder.Property(e => e.QuestionsCount)
    .HasDefaultValue(0)
    .IsRequired();
```

---

## Issue 5: Grades Cannot Be Negative

### Solution Implemented:

#### 1. **Range Validation** (`StudentExamGrade.cs`)
```csharp
[Range(0, double.MaxValue, ErrorMessage = "Grade cannot be negative.")]
public double Grade { get; set; }
```

#### 2. **Database Configuration** (`StudentExamGradeConfig.cs`)
```csharp
builder.Property(seg => seg.Grade)
    .HasPrecision(18, 2)
    .IsRequired();
```

---

## Issue 6: Grades Cannot Exceed Exam Full Marks

### Solution Implemented:

#### 1. **Method-Level Validation** (`StudentExamGrade.cs`)
```csharp
public bool IsValid()
{
    if (Exam != null && Grade > Exam.Fullmark)
    {
        return false;
    }
    return true;
}
```

#### 2. **Service-Level Validation** (`ExamValidationService.cs`)
```csharp
public static ValidationResult ValidateStudentExamGrade(StudentExamGrade grade)
{
    if (grade.Exam != null && grade.Grade > grade.Exam.Fullmark)
    {
        return new ValidationResult(
            $"Grade ({grade.Grade}) cannot exceed exam full marks ({grade.Exam.Fullmark})."
        );
    }
    return ValidationResult.Success;
}
```

---

## Validation Layers

### 1. **Data Annotations Layer** (Automatic)
- Runs automatically when `ModelState.IsValid` is checked
- Provides basic range and format validation
- Executed by ASP.NET Core framework

### 2. **Database Constraint Layer** (Enforced)
- Prevents invalid data from being saved to database
- Applied in `*Config.cs` files via Fluent API
- Ensures data integrity at the database level

### 3. **Service Layer** (Business Logic)
- Provides custom validation for complex rules
- Used in controllers before saving to database
- Allows detailed error messages

### 4. **Model Method Layer** (Reusable)
- `StudentExamGrade.IsValid()` method
- Can be called from any layer
- Keeps validation logic close to data

---

## Usage Examples

### Example 1: Creating an Exam (Controller)

```csharp
[HttpPost]
public async Task<IActionResult> CreateExam(Exam exam)
{
    // Step 1: Check model state (data annotations)
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Step 2: Validate using service
    var validationResult = ExamValidationService.ValidateExam(exam);
    if (!validationResult.Succeeded)
    {
        return BadRequest(validationResult.ErrorMessage);
    }

    // Step 3: Save to database (database constraints enforced)
    _context.Exams.Add(exam);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetExam), new { id = exam.Id }, exam);
}
```

### Example 2: Assigning Grade (Controller)

```csharp
[HttpPost("grade")]
public async Task<IActionResult> AssignGrade(StudentExamGrade grade)
{
    // Step 1: Check model state
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Step 2: Fetch exam to validate grade against fullmarks
    var exam = await _context.Exams.FindAsync(grade.ExamId);
    if (exam == null)
    {
        return NotFound("Exam not found.");
    }

    grade.Exam = exam;

    // Step 3: Validate grade
    var validationResult = ExamValidationService.ValidateStudentExamGrade(grade);
    if (!validationResult.Succeeded)
    {
        return BadRequest(validationResult.ErrorMessage);
    }

    // Step 4: Save to database
    _context.StudentExamGrades.Add(grade);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetGrade), new { id = grade.Id }, grade);
}
```

### Example 3: Using Model Method

```csharp
var grade = new StudentExamGrade 
{ 
    Grade = 95,
    Exam = exam 
};

if (!grade.IsValid())
{
    // Handle invalid grade
}
```

---

## Testing Validation

### Invalid Exam Date (Past)
```
Error: "Exam date must be in the future."
```

### Invalid Duration (0 or negative)
```
Error: "Duration must be at least 1 minute."
```

### Invalid Grade (Exceeds Full Marks)
```
Error: "Grade (95) cannot exceed exam full marks (50)."
```

### Invalid Grade (Negative)
```
Error: "Grade cannot be negative."
```

---

## Key Benefits

✅ **Multi-layer validation** - Data validated at multiple levels  
✅ **Automatic enforcement** - Data annotations run automatically  
✅ **Database integrity** - Constraints prevent invalid data at the source  
✅ **Clear error messages** - Users get specific, actionable feedback  
✅ **Reusable validation** - Service layer can be used across the application  
✅ **Type-safe** - Validation rules are strongly typed  
✅ **Future-proof** - Easy to extend with additional validation rules  

---

## Summary

| Issue | Type | Layer | Status |
|-------|------|-------|--------|
| Exam date in past | Business Rule | Data Annotation + Custom | ✅ Fixed |
| Duration ≤ 0 | Data Range | Data Annotation + DB | ✅ Fixed |
| Full marks ≤ 0 | Data Range | Data Annotation + DB | ✅ Fixed |
| Questions ≤ 0 | Data Range | Data Annotation + DB | ✅ Fixed |
| Grade < 0 | Data Range | Data Annotation + DB | ✅ Fixed |
| Grade > Fullmark | Business Rule | Model + Service | ✅ Fixed |

