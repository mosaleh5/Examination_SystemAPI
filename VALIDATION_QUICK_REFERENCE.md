## Quick Validation Reference

### Data Annotation Validators Used

```csharp
// 1. Exam Date - Must be in future
[FutureDate(ErrorMessage = "Exam date must be in the future.")]
public DateTime Date { get; set; }

// 2. Duration - Must be positive (1+ minutes)
[Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
public int DurationMinutes { get; set; }

// 3. Full Marks - Must be positive
[Range(1, int.MaxValue, ErrorMessage = "Full marks must be greater than 0.")]
public int Fullmark { get; set; }

// 4. Questions Count - Must be positive
[Range(1, int.MaxValue, ErrorMessage = "Questions count must be at least 1.")]
public int QuestionsCount { get; set; }

// 5. Grade - Cannot be negative
[Range(0, double.MaxValue, ErrorMessage = "Grade cannot be negative.")]
public double Grade { get; set; }

// 6. Title - Max length
[MaxLength(150)]
public string Title { get; set; }
```

### Validation Service Methods

```csharp
// Validate entire exam
ExamValidationService.ValidateExam(exam);

// Validate student grade
ExamValidationService.ValidateStudentExamGrade(grade);
```

### Model Validation Method

```csharp
// Check if grade is valid against exam fullmark
if (grade.IsValid()) { /* OK */ }
```

### Controller Template

```csharp
[HttpPost]
public async Task<IActionResult> CreateExam(Exam exam)
{
    // 1. Validate model state (data annotations)
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    // 2. Validate business logic (service)
    var result = ExamValidationService.ValidateExam(exam);
    if (!result.Succeeded)
        return BadRequest(result.ErrorMessage);
    
    // 3. Save (database constraints)
    _context.Exams.Add(exam);
    await _context.SaveChangesAsync();
    
    return CreatedAtAction(nameof(GetExam), new { id = exam.Id }, exam);
}
```

### Error Response Examples

```json
// Invalid date
{
  "error": "Exam date must be in the future."
}

// Invalid duration
{
  "error": "Duration must be at least 1 minute."
}

// Grade exceeds fullmark
{
  "error": "Grade (95) cannot exceed exam full marks (50)."
}

// Multiple validation errors
{
  "Date": ["Exam date must be in the future."],
  "DurationMinutes": ["Duration must be at least 1 minute."]
}
```

