# DTO Implementation Guide

## Overview
This guide explains how DTOs (Data Transfer Objects) are implemented in the Examination System's service and repository layers.

## Architecture

```
Controller Layer (DTOs)
    ↓
Service Layer (DTOs ↔ Entities)
    ↓
Repository Layer (Entities)
    ↓
Database
```

## Why Use DTOs?

### 1. **Separation of Concerns**
- External API contracts (DTOs) are separate from internal data models (Entities)
- Changes to database schema don't break API contracts

### 2. **Security**
- Hide sensitive data (e.g., passwords, internal IDs)
- Expose only what clients need

### 3. **Performance**
- Return only required fields
- Reduce payload size

### 4. **Validation**
- Different validation rules for Create/Update operations
- API-specific validation attributes

### 5. **Versioning**
- Easier to maintain multiple API versions

## Folder Structure

```
Examination_System/
├── DTOs/
│   ├── Course/
│   │   ├── CourseDto.cs              # Basic course info
│   │   ├── CreateCourseDto.cs        # For POST requests
│   │   ├── UpdateCourseDto.cs        # For PUT/PATCH requests
│   │   └── CourseDetailsDto.cs       # Detailed info with relations
│   ├── Student/
│   ├── Exam/
│   └── ...
├── Mappings/
│   ├── CourseMapper.cs               # Entity ↔ DTO conversions
│   ├── StudentMapper.cs
│   └── ...
├── Services/
│   ├── CourseServices.cs             # Business logic with DTOs
│   └── ...
├── Controllers/
│   ├── CourseController.cs           # API endpoints using DTOs
│   └── ...
└── Repository/
    ├── GenericRepository.cs          # Data access with Entities
    └── ...
```

## DTO Types

### 1. **Basic DTO** (`CourseDto`)
```csharp
// Used for: GET list, simple responses
public class CourseDto
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string InstructorName { get; set; }
    // ... only essential fields
}
```

### 2. **Create DTO** (`CreateCourseDto`)
```csharp
// Used for: POST (create new)
public class CreateCourseDto
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public int InstructorId { get; set; }
    // ... no ID field (auto-generated)
}
```

### 3. **Update DTO** (`UpdateCourseDto`)
```csharp
// Used for: PUT/PATCH (update existing)
public class UpdateCourseDto
{
    [Required]
    public int ID { get; set; }
    
    public string Name { get; set; }  // Optional fields
    public int? InstructorId { get; set; }
}
```

### 4. **Details DTO** (`CourseDetailsDto`)
```csharp
// Used for: GET by ID (detailed view)
public class CourseDetailsDto : CourseDto
{
    public List<ExamDto> Exams { get; set; }
    public List<StudentDto> EnrolledStudents { get; set; }
    // ... includes related entities
}
```

## Mapper Pattern

### Purpose
Converts between Entities and DTOs

### Implementation
```csharp
public static class CourseMapper
{
    // Entity → DTO
    public static CourseDto ToDto(Course course)
    {
        return new CourseDto
        {
            ID = course.ID,
            Name = course.Name,
            InstructorName = $"{course.Instructor.User.FirstName} {course.Instructor.User.LastName}"
        };
    }

    // DTO → Entity (for Create)
    public static Course ToEntity(CreateCourseDto dto)
    {
        return new Course
        {
            Name = dto.Name,
            InstructorId = dto.InstructorId,
            CreatedAt = DateTime.UtcNow
        };
    }

    // Update existing entity from DTO
    public static void UpdateEntity(Course course, UpdateCourseDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Name))
            course.Name = dto.Name;
        // ... update only provided fields
    }
}
```

## Service Layer Pattern

### Service Methods Using DTOs

```csharp
public class CourseServices
{
    private readonly UnitOfWork _unitOfWork;

    // GET All → Returns List<DTO>
    public async Task<IEnumerable<CourseDto>> GetAllAsync(CourseSpecifications spec)
    {
        var courses = await _unitOfWork.Repository<Course>()
            .GetAllWithSpecificationAsync(spec);
        
        return CourseMapper.ToDtoList(courses);  // Entity → DTO
    }

    // GET By ID → Returns Detailed DTO
    public async Task<CourseDetailsDto> GetByIdAsync(int id)
    {
        var spec = new CourseSpecifications(id);
        var course = await _unitOfWork.Repository<Course>()
            .GetByIdWithSpecification(spec);
        
        return CourseMapper.ToDetailsDto(course);  // Entity → DTO
    }

    // POST Create → Accepts Create DTO, Returns DTO
    public async Task<CourseDto> CreateAsync(CreateCourseDto createDto)
    {
        // 1. Validate
        var instructor = await _unitOfWork.Repository<Instructor>()
            .GetByIdAsync(createDto.InstructorId);
        if (instructor == null)
            throw new InvalidOperationException("Instructor not found");

        // 2. Convert DTO → Entity
        var course = CourseMapper.ToEntity(createDto);
        
        // 3. Save to database
        var createdCourse = await _unitOfWork.Repository<Course>()
            .AddAsync(course);
        await _unitOfWork.CompleteAsync();

        // 4. Reload with related data
        var spec = new CourseSpecifications(createdCourse.ID);
        var courseWithDetails = await _unitOfWork.Repository<Course>()
            .GetByIdWithSpecification(spec);
        
        // 5. Convert Entity → DTO
        return CourseMapper.ToDto(courseWithDetails);
    }

    // PUT Update → Accepts Update DTO, Returns DTO
    public async Task<CourseDto> UpdateAsync(int id, UpdateCourseDto updateDto)
    {
        // 1. Validate ID
        if (id != updateDto.ID)
            throw new ArgumentException("ID mismatch");

        // 2. Load existing entity
        var course = await _unitOfWork.Repository<Course>()
            .GetByIdAsync(id);
        if (course == null)
            throw new InvalidOperationException("Course not found");

        // 3. Validate related entities
        if (updateDto.InstructorId.HasValue)
        {
            var instructor = await _unitOfWork.Repository<Instructor>()
                .GetByIdAsync(updateDto.InstructorId.Value);
            if (instructor == null)
                throw new InvalidOperationException("Instructor not found");
        }

        // 4. Update entity from DTO
        CourseMapper.UpdateEntity(course, updateDto);
        
        // 5. Save changes
        var updatedCourse = await _unitOfWork.Repository<Course>()
            .UpdateAsync(course);
        await _unitOfWork.CompleteAsync();

        // 6. Reload with related data
        var spec = new CourseSpecifications(updatedCourse.ID);
        var courseWithDetails = await _unitOfWork.Repository<Course>()
            .GetByIdWithSpecification(spec);
        
        // 7. Convert Entity → DTO
        return CourseMapper.ToDto(courseWithDetails);
    }

    // DELETE
    public async Task<bool> DeleteAsync(int id)
    {
        var course = await _unitOfWork.Repository<Course>()
            .GetByIdAsync(id);
        if (course == null)
            return false;

        // Soft delete
        course.IsDeleted = true;
        await _unitOfWork.Repository<Course>().UpdateAsync(course);
        await _unitOfWork.CompleteAsync();

        return true;
    }
}
```

## Repository Layer Pattern

### Repository Always Works with Entities

```csharp
public class GenericRepository<T> where T : BaseModel
{
    // ❌ NEVER expose DTOs in repository
    // ✅ ALWAYS work with entities
    
    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>()
            .FirstOrDefaultAsync(x => x.ID == id && !x.IsDeleted);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>()
            .Where(x => !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    // ... other methods work with entities
}
```

### Why Repository Uses Entities?

1. **Single Responsibility**: Repository handles data access only
2. **Reusability**: Same repository can be used by multiple services
3. **Testability**: Easier to mock and test
4. **EF Core Integration**: EF Core works with entities, not DTOs

## Controller Layer Pattern

### Controllers Use DTOs for API Contracts

```csharp
[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly CourseServices _courseServices;

    // GET api/course
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CourseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll()
    {
        var spec = new CourseSpecifications();
        var courses = await _courseServices.GetAllAsync(spec);
        return Ok(courses);  // Returns DTOs
    }

    // GET api/course/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CourseDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDetailsDto>> GetById(int id)
    {
        var course = await _courseServices.GetByIdAsync(id);
        
        if (course == null)
            return NotFound(new { message = $"Course with ID {id} not found" });

        return Ok(course);  // Returns DTO
    }

    // POST api/course
    [HttpPost]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var course = await _courseServices.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = course.ID }, course);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT api/course/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> Update(int id, [FromBody] UpdateCourseDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != updateDto.ID)
            return BadRequest(new { message = "ID mismatch" });

        try
        {
            var updatedCourse = await _courseServices.UpdateAsync(id, updateDto);
            return Ok(updatedCourse);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // DELETE api/course/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _courseServices.DeleteAsync(id);
        
        if (!result)
            return NotFound();

        return NoContent();
    }
}
```

## Best Practices

### 1. **Naming Conventions**
- `EntityDto` - Basic DTO
- `CreateEntityDto` - For POST
- `UpdateEntityDto` - For PUT/PATCH
- `EntityDetailsDto` - Detailed view
- `EntityListDto` - For paginated lists

### 2. **Validation**
```csharp
public class CreateCourseDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid instructor ID")]
    public int InstructorId { get; set; }
}
```

### 3. **Null Safety**
```csharp
public static CourseDto ToDto(Course course)
{
    if (course == null) return null;  // Guard clause
    
    return new CourseDto
    {
        InstructorName = course.Instructor?.User?.FirstName ?? "N/A"  // Null coalescing
    };
}
```

### 4. **Async/Await Pattern**
```csharp
public async Task<CourseDto> GetByIdAsync(int id)
{
    // ✅ Good: Await all async operations
    var spec = new CourseSpecifications(id);
    var course = await _unitOfWork.Repository<Course>()
        .GetByIdWithSpecification(spec);
    
    return CourseMapper.ToDto(course);
    
    // ❌ Bad: Blocking async operations
    // var course = _unitOfWork.Repository<Course>()
    //     .GetByIdWithSpecification(spec).Result;
}
```

### 5. **Error Handling**
```csharp
public async Task<CourseDto> CreateAsync(CreateCourseDto dto)
{
    // Validate business rules
    if (dto == null)
        throw new ArgumentNullException(nameof(dto));
    
    // Validate related entities
    var instructor = await _unitOfWork.Repository<Instructor>()
        .GetByIdAsync(dto.InstructorId);
    if (instructor == null)
        throw new InvalidOperationException($"Instructor with ID {dto.InstructorId} not found");
    
    // Proceed with creation
    // ...
}
```

### 6. **Return Types**
```csharp
// ✅ Good: Return DTOs from service layer
public async Task<CourseDto> GetByIdAsync(int id)
{
    var course = await _repository.GetByIdAsync(id);
    return CourseMapper.ToDto(course);
}

// ❌ Bad: Return entities from service layer
// public async Task<Course> GetByIdAsync(int id)
// {
//     return await _repository.GetByIdAsync(id);
// }
```

## Data Flow Examples

### Example 1: GET Request
```
1. Client → GET /api/course/1
2. Controller → CourseServices.GetByIdAsync(1)
3. Service → Repository.GetByIdWithSpecification(spec)
4. Repository → Database (returns Course entity)
5. Service → CourseMapper.ToDetailsDto(course)
6. Controller → Ok(courseDto)
7. Client ← JSON response (CourseDetailsDto)
```

### Example 2: POST Request
```
1. Client → POST /api/course + CreateCourseDto JSON
2. Controller → validates ModelState
3. Controller → CourseServices.CreateAsync(createDto)
4. Service → validates business rules
5. Service → CourseMapper.ToEntity(createDto)
6. Service → Repository.AddAsync(course)
7. Repository → Database (saves Course entity)
8. Service → Repository.GetByIdWithSpecification(newId)
9. Service → CourseMapper.ToDto(course)
10. Controller → CreatedAtAction(..., courseDto)
11. Client ← 201 Created + JSON response (CourseDto)
```

### Example 3: PUT Request
```
1. Client → PUT /api/course/1 + UpdateCourseDto JSON
2. Controller → validates ModelState and ID match
3. Controller → CourseServices.UpdateAsync(1, updateDto)
4. Service → validates business rules
5. Service → Repository.GetByIdAsync(1)
6. Service → CourseMapper.UpdateEntity(course, updateDto)
7. Service → Repository.UpdateAsync(course)
8. Repository → Database (updates Course entity)
9. Service → Repository.GetByIdWithSpecification(1)
10. Service → CourseMapper.ToDto(course)
11. Controller → Ok(courseDto)
12. Client ← 200 OK + JSON response (CourseDto)
```

## Common Mistakes to Avoid

### ❌ Mistake 1: Exposing Entities in Controller
```csharp
// BAD: Controller returns entity
[HttpGet("{id}")]
public async Task<Course> GetById(int id)
{
    return await _service.GetByIdAsync(id);
}
```

### ✅ Correct Approach
```csharp
// GOOD: Controller returns DTO
[HttpGet("{id}")]
public async Task<ActionResult<CourseDto>> GetById(int id)
{
    var course = await _service.GetByIdAsync(id);
    return Ok(course);
}
```

### ❌ Mistake 2: Using DTOs in Repository
```csharp
// BAD: Repository works with DTOs
public async Task<CourseDto> GetByIdAsync(int id)
{
    var course = await _context.Courses.FindAsync(id);
    return CourseMapper.ToDto(course);
}
```

### ✅ Correct Approach
```csharp
// GOOD: Repository works with entities
public async Task<Course> GetByIdAsync(int id)
{
    return await _context.Courses.FindAsync(id);
}

// Service layer converts to DTO
public async Task<CourseDto> GetByIdAsync(int id)
{
    var course = await _repository.GetByIdAsync(id);
    return CourseMapper.ToDto(course);
}
```

### ❌ Mistake 3: Not Validating DTOs
```csharp
// BAD: No validation
[HttpPost]
public async Task<CourseDto> Create(CreateCourseDto dto)
{
    return await _service.CreateAsync(dto);
}
```

### ✅ Correct Approach
```csharp
// GOOD: Validate ModelState
[HttpPost]
public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    return Ok(await _service.CreateAsync(dto));
}
```

## Testing DTOs

### Unit Test Example
```csharp
[Fact]
public void CourseMapper_ToDto_MapsCorrectly()
{
    // Arrange
    var course = new Course
    {
        ID = 1,
        Name = "Test Course",
        Instructor = new Instructor
        {
            User = new User
            {
                FirstName = "John",
                LastName = "Doe"
            }
        }
    };

    // Act
    var dto = CourseMapper.ToDto(course);

    // Assert
    Assert.NotNull(dto);
    Assert.Equal(1, dto.ID);
    Assert.Equal("Test Course", dto.Name);
    Assert.Equal("John Doe", dto.InstructorName);
}
```

## Alternative: AutoMapper

If you prefer automated mapping, you can use AutoMapper:

```csharp
// Install: Install-Package AutoMapper

// Configure mappings
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.InstructorName, 
                opt => opt.MapFrom(src => $"{src.Instructor.User.FirstName} {src.Instructor.User.LastName}"));
        
        CreateMap<CreateCourseDto, Course>();
    }
}

// Use in service
public class CourseServices
{
    private readonly IMapper _mapper;
    
    public async Task<CourseDto> GetByIdAsync(int id)
    {
        var course = await _repository.GetByIdAsync(id);
        return _mapper.Map<CourseDto>(course);
    }
}
```

## Summary

| Layer | Works With | Purpose |
|-------|------------|---------|
| **Controller** | DTOs | API contracts, HTTP concerns |
| **Service** | DTOs + Entities | Business logic, mapping |
| **Repository** | Entities | Data access only |
| **Database** | Entities | Persistence |

**Key Principle**: DTOs are for external communication, Entities are for internal data management.
