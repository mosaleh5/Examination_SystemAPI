# DTO Quick Start Guide

## What You Have Now

✅ **DTOs Created** for Course entity:
- `CourseDto` - Basic information
- `CreateCourseDto` - For creating new courses
- `UpdateCourseDto` - For updating courses
- `CourseDetailsDto` - Detailed view with relations

✅ **Mapper Created**: `CourseMapper` - Converts between entities and DTOs

✅ **Service Updated**: `CourseServices` - Now uses DTOs

✅ **Controller Updated**: `CourseController` - REST API with DTOs

## How to Test

### 1. Get All Courses
```http
GET http://localhost:5000/api/course
```

**Response**:
```json
[
  {
    "id": 1,
    "name": "Introduction to Programming",
    "description": "Learn the fundamentals...",
    "hours": "40",
    "instructorId": 1,
    "instructorName": "Ahmed Mohammed",
    "createdAt": "2024-12-13T09:00:00Z"
  }
]
```

### 2. Get Course by ID
```http
GET http://localhost:5000/api/course/1
```

**Response**:
```json
{
  "id": 1,
  "name": "Introduction to Programming",
  "description": "Learn the fundamentals...",
  "hours": "40",
  "createdAt": "2024-12-13T09:00:00Z",
  "instructorId": 1,
  "instructorName": "Ahmed Mohammed",
  "instructorEmail": "ahmed.mohammed@example.com",
  "enrolledStudentsCount": 4,
  "examsCount": 2,
  "exams": [
    {
      "id": 1,
      "title": "Programming Midterm Exam",
      "date": "2025-01-15T10:00:00Z",
      "durationMinutes": 90,
      "examType": "Final"
    }
  ],
  "enrolledStudents": [
    {
      "studentId": 1,
      "studentName": "Mohammed Ibrahim",
      "major": "Computer Science",
      "enrollmentDate": "2024-09-01T00:00:00Z"
    }
  ]
}
```

### 3. Create New Course
```http
POST http://localhost:5000/api/course
Content-Type: application/json

{
  "name": "Advanced C# Programming",
  "description": "Master advanced C# concepts and patterns",
  "hours": "60",
  "instructorId": 1
}
```

**Response**: `201 Created`
```json
{
  "id": 6,
  "name": "Advanced C# Programming",
  "description": "Master advanced C# concepts and patterns",
  "hours": "60",
  "instructorId": 1,
  "instructorName": "Ahmed Mohammed",
  "createdAt": "2025-01-10T10:00:00Z"
}
```

### 4. Update Course
```http
PUT http://localhost:5000/api/course/6
Content-Type: application/json

{
  "id": 6,
  "name": "Advanced C# Programming Updated",
  "description": "Updated description",
  "hours": "65",
  "instructorId": 2
}
```

**Response**: `200 OK`
```json
{
  "id": 6,
  "name": "Advanced C# Programming Updated",
  "description": "Updated description",
  "hours": "65",
  "instructorId": 2,
  "instructorName": "Fatima Ali",
  "createdAt": "2025-01-10T10:00:00Z"
}
```

### 5. Delete Course
```http
DELETE http://localhost:5000/api/course/6
```

**Response**: `204 No Content`

## API Endpoints Summary

| Method | Endpoint | Description | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| GET | `/api/course` | Get all courses | - | `CourseDto[]` |
| GET | `/api/course/{id}` | Get course by ID | - | `CourseDetailsDto` |
| GET | `/api/course/instructor/{id}` | Get courses by instructor | - | `CourseDto[]` |
| POST | `/api/course` | Create new course | `CreateCourseDto` | `CourseDto` |
| PUT | `/api/course/{id}` | Update course | `UpdateCourseDto` | `CourseDto` |
| DELETE | `/api/course/{id}` | Delete course | - | `204 No Content` |
| GET | `/api/course/{id}/exists` | Check if exists | - | `{ "exists": true }` |
| GET | `/api/course/{id}/students/count` | Get enrolled count | - | `{ "courseId": 1, "enrolledStudentsCount": 4 }` |

## Next Steps: Implement DTOs for Other Entities

### 1. Student DTOs
```csharp
// DTOs/Student/StudentDto.cs
public class StudentDto
{
    public int ID { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Major { get; set; }
    public DateTime EnrollmentDate { get; set; }
}

// DTOs/Student/CreateStudentDto.cs
public class CreateStudentDto
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    public string Phone { get; set; }
    [Required]
    public string Major { get; set; }
}
```

### 2. Exam DTOs
```csharp
// DTOs/Exam/ExamDto.cs
public class ExamDto
{
    public int ID { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public int DurationMinutes { get; set; }
    public int Fullmark { get; set; }
    public string ExamType { get; set; }
    public int QuestionsCount { get; set; }
    public string CourseName { get; set; }
    public string InstructorName { get; set; }
}

// DTOs/Exam/CreateExamDto.cs
public class CreateExamDto
{
    [Required]
    public string Title { get; set; }
    [Required]
    [FutureDate]
    public DateTime Date { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int Fullmark { get; set; }
    [Required]
    public ExamType ExamType { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int QuestionsCount { get; set; }
    [Required]
    public int CourseId { get; set; }
    [Required]
    public int InstructorId { get; set; }
}
```

### 3. Question DTOs
```csharp
// DTOs/Question/QuestionDto.cs
public class QuestionDto
{
    public int ID { get; set; }
    public string Title { get; set; }
    public int Mark { get; set; }
    public string Level { get; set; }
    public int InstructorId { get; set; }
    public List<ChoiceDto> Choices { get; set; }
}

// DTOs/Question/CreateQuestionDto.cs
public class CreateQuestionDto
{
    [Required]
    [MaxLength(500)]
    public string Title { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int Mark { get; set; }
    [Required]
    public QuestionLevel Level { get; set; }
    [Required]
    public int InstructorId { get; set; }
    public List<CreateChoiceDto> Choices { get; set; }
}

// DTOs/Question/ChoiceDto.cs
public class ChoiceDto
{
    public int ID { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}

// DTOs/Question/CreateChoiceDto.cs
public class CreateChoiceDto
{
    [Required]
    public string Text { get; set; }
    [Required]
    public bool IsCorrect { get; set; }
}
```

## Pattern to Follow

For each entity, create:

1. **DTOs** (`DTOs/{EntityName}/`)
   - `{EntityName}Dto.cs` - Basic info
   - `Create{EntityName}Dto.cs` - For POST
   - `Update{EntityName}Dto.cs` - For PUT
   - `{EntityName}DetailsDto.cs` - With relations

2. **Mapper** (`Mappings/{EntityName}Mapper.cs`)
   ```csharp
   public static class {EntityName}Mapper
   {
       public static {EntityName}Dto ToDto({EntityName} entity) { }
       public static {EntityName}DetailsDto ToDetailsDto({EntityName} entity) { }
       public static {EntityName} ToEntity(Create{EntityName}Dto dto) { }
       public static void UpdateEntity({EntityName} entity, Update{EntityName}Dto dto) { }
       public static IEnumerable<{EntityName}Dto> ToDtoList(IEnumerable<{EntityName}> entities) { }
   }
   ```

3. **Service** (`Services/{EntityName}Services.cs`)
   ```csharp
   public class {EntityName}Services
   {
       public async Task<IEnumerable<{EntityName}Dto>> GetAllAsync() { }
       public async Task<{EntityName}DetailsDto> GetByIdAsync(int id) { }
       public async Task<{EntityName}Dto> CreateAsync(Create{EntityName}Dto dto) { }
       public async Task<{EntityName}Dto> UpdateAsync(int id, Update{EntityName}Dto dto) { }
       public async Task<bool> DeleteAsync(int id) { }
   }
   ```

4. **Controller** (`Controllers/{EntityName}Controller.cs`)
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class {EntityName}Controller : ControllerBase
   {
       [HttpGet]
       public async Task<ActionResult<IEnumerable<{EntityName}Dto>>> GetAll() { }
       
       [HttpGet("{id}")]
       public async Task<ActionResult<{EntityName}DetailsDto>> GetById(int id) { }
       
       [HttpPost]
       public async Task<ActionResult<{EntityName}Dto>> Create([FromBody] Create{EntityName}Dto dto) { }
       
       [HttpPut("{id}")]
       public async Task<ActionResult<{EntityName}Dto>> Update(int id, [FromBody] Update{EntityName}Dto dto) { }
       
       [HttpDelete("{id}")]
       public async Task<IActionResult> Delete(int id) { }
   }
   ```

## Benefits You Get

✅ **Security**: Don't expose sensitive data or internal IDs
✅ **Validation**: Different rules for Create/Update
✅ **Performance**: Return only needed fields
✅ **Flexibility**: API independent from database schema
✅ **Versioning**: Easy to create API v2 without changing entities
✅ **Documentation**: Clear API contracts

## Files Created

```
Examination_System/
├── DTOs/
│   └── Course/
│       ├── CourseDto.cs
│       ├── CreateCourseDto.cs
│       ├── UpdateCourseDto.cs
│       └── CourseDetailsDto.cs
├── Mappings/
│   └── CourseMapper.cs
├── Services/
│   └── CourseServices.cs (updated)
└── Controllers/
    └── CourseController.cs (updated)
```

## Documentation Files Created

- `DTO_IMPLEMENTATION_GUIDE.md` - Complete implementation guide
- `DTO_QUICK_START.md` - This file (quick reference)

## Summary

Your application now follows the **DTO pattern** properly:

1. **Controller** receives/returns DTOs
2. **Service** converts between DTOs and Entities
3. **Repository** works only with Entities
4. **Database** stores Entities

This is the industry-standard approach for building maintainable, secure, and scalable APIs! 🎉
