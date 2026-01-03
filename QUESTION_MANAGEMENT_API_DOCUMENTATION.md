# Question Management API - Edit & Delete Endpoints

## Overview
This document describes the newly implemented endpoints for editing and deleting questions in the Examination System API.

## Architecture

### Layers Implemented
1. **ViewModels** - `UpdateQuestionViewModel.cs`
2. **DTOs** - `UpdateQuestionDto.cs`
3. **Services** - `QuestionServices.cs` (Updated)
4. **Controllers** - `QuestionController.cs` (Updated)
5. **Mappings** - `MappingProfiles.cs` (Updated)

### Features
- ✅ Soft Delete Implementation
- ✅ Unit of Work Pattern
- ✅ Repository Pattern
- ✅ AutoMapper Integration
- ✅ Data Validation
- ✅ Authorization (Instructor only)
- ✅ Ownership Verification

---

## API Endpoints

### 1. Get Question By ID
**Endpoint:** `GET /api/Question/question/{questionId}`

**Authorization:** Instructor Role Required

**Description:** Retrieves a specific question by ID, ensuring the instructor owns the question.

**Response:**
```json
{
  "id": 1,
  "title": "What is polymorphism?",
  "mark": 10,
  "level": 1,
  "courseId": 5,
  "choices": [
    {
      "text": "Ability of objects to take many forms",
      "isCorrect": true,
      "questionId": 1
    },
    {
      "text": "A type of inheritance",
      "isCorrect": false,
      "questionId": 1
    }
  ]
}
```

**Status Codes:**
- `200 OK` - Question found
- `401 Unauthorized` - User not authenticated
- `404 Not Found` - Question not found or no permission
- `500 Internal Server Error` - Server error

---

### 2. Update Question
**Endpoint:** `PUT /api/Question/{questionId}`

**Authorization:** Instructor Role Required

**Description:** Updates an existing question. Only the question owner can update it.

**Request Body:**
```json
{
  "id": 1086,
  "title": "What is a variable in programming? (Updated)",
  "mark": 8,
  "level": 0,
  "courseId": 9,
  "choices": [
    {
      "text": "A named storage location in memory",
      "isCorrect": true
    },
    {
      "text": "A type of loop",
      "isCorrect": false
    },
    {
      "text": "A function definition",
      "isCorrect": false
    }
  ]
}
```

**Validation Rules:**
- `id` - Required, must match URL parameter
- `title` - Required, max 500 characters
- `mark` - Required, range 1-100
- `level` - Required (Easy=0, Medium=1, Hard=2)
- `courseId` - Required
- `choices` - Required, minimum 2 choices
- **Only ONE choice can be marked as correct**

**Response:**
```json
{
  "id": 1086,
  "title": "What is a variable in programming? (Updated)",
  "mark": 8,
  "level": 0,
  "courseId": 9,
  "choices": [
    {
      "id": 2501,
      "text": "A named storage location in memory",
      "isCorrect": true,
      "questionId": 1086
    },
    {
      "id": 2502,
      "text": "A type of loop",
      "isCorrect": false,
      "questionId": 1086
    },
    {
      "id": 2503,
      "text": "A function definition",
      "isCorrect": false,
      "questionId": 1086
    }
  ]
}
```

**Status Codes:**
- `200 OK` - Question updated successfully
- `400 Bad Request` - Validation failed or ID mismatch
- `401 Unauthorized` - User not authenticated
- `404 Not Found` - Question not found or no permission
- `500 Internal Server Error` - Server error

**Important Notes:**
- All existing choices are deleted and replaced with new ones
- Course existence is validated before update
- Instructor ownership is verified

---

### 3. Delete Question (Soft Delete)
**Endpoint:** `DELETE /api/Question/{questionId}`

**Authorization:** Instructor Role Required

**Description:** Soft deletes a question. The question is marked as deleted but not removed from the database.

**Response:**
```json
{
  "message": "Question deleted successfully.",
  "questionId": 1086
}
```

**Status Codes:**
- `200 OK` - Question deleted successfully
- `400 Bad Request` - Invalid question ID or question is used in exams
- `401 Unauthorized` - User not authenticated
- `404 Not Found` - Question not found or no permission
- `500 Internal Server Error` - Server error

**Important Notes:**
- **Soft Delete:** Question is marked as deleted (`IsDeleted = true`)
- **Validation:** Cannot delete questions that are used in active exams
- **Ownership:** Only the question owner can delete it

**Error Scenarios:**
```json
{
  "message": "Cannot delete question with ID 1086 because it is used in one or more exams."
}
```

---

## Testing Examples

### Test 1: Get Question
```bash
curl -X GET "https://localhost:5001/api/Question/question/1086" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Test 2: Update Question
```bash
curl -X PUT "https://localhost:5001/api/Question/1086" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1086,
    "title": "Updated Question Title",
    "mark": 10,
    "level": 1,
    "courseId": 9,
    "choices": [
      {
        "text": "Choice A",
        "isCorrect": true
      },
      {
        "text": "Choice B",
        "isCorrect": false
      }
    ]
  }'
```

### Test 3: Delete Question
```bash
curl -X DELETE "https://localhost:5001/api/Question/1086" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## Service Layer Implementation

### Key Methods

#### GetQuestionByIdAsync
```csharp
Task<QuestionToReturnDto> GetQuestionByIdAsync(int questionId, string instructorId)
```
- Validates instructor ownership
- Uses specifications for filtering
- Returns mapped DTO

#### UpdateQuestionAsync
```csharp
Task<QuestionToReturnDto> UpdateQuestionAsync(UpdateQuestionDto updateQuestionDto)
```
- Validates ownership and course existence
- Deletes old choices (soft delete)
- Creates new choices
- Updates question properties
- Returns updated question

#### DeleteQuestionAsync
```csharp
Task<bool> DeleteQuestionAsync(int questionId, string instructorId)
```
- Validates ownership
- Checks if question is used in exams
- Performs soft delete
- Returns success status

---

## Database Impact

### Soft Delete Behavior
When a question is deleted:
1. `IsDeleted` flag is set to `true`
2. `DeletedAt` timestamp is recorded (from BaseModel)
3. Question remains in database
4. Filtered out from normal queries via specifications

### Cascade Operations
- **Choices:** Soft deleted when question is updated or deleted
- **ExamQuestions:** Validation prevents deletion if question is used in exams

---

## Security Features

1. **Role-Based Authorization:** Only instructors can manage questions
2. **Ownership Verification:** Instructors can only modify their own questions
3. **JWT Token Required:** All endpoints require valid authentication
4. **Input Validation:** Comprehensive validation on all inputs

---

## Error Handling

All endpoints implement comprehensive error handling:

```csharp
try
{
    // Operation
}
catch (KeyNotFoundException ex)
{
    return NotFound(new { message = ex.Message });
}
catch (InvalidOperationException ex)
{
    return BadRequest(new { message = ex.Message });
}
catch (Exception ex)
{
    return StatusCode(500, new { message = "Error", details = ex.Message });
}
```

---

## Files Modified/Created

### Created Files
1. `Examination_System\ViewModels\Question\UpdateQuestionViewModel.cs`
2. `Examination_System\DTOs\Question\UpdateQuestionDto.cs`

### Modified Files
1. `Examination_System\Controllers\QuestionController.cs`
2. `Examination_System\Services\QuestionServices\IQuestionServices.cs`
3. `Examination_System\Services\QuestionServices\QuestionServices.cs`
4. `Examination_System\Helpers\MappingProfiles.cs`
5. `Examination_System\Controllers\CourseController.cs` (Bug fix)
6. `Examination_System\Services\CourseServices\CourseServices.cs` (Bug fix)

---

## Additional Improvements Made

1. **Fixed CourseController syntax error** - Missing closing brace
2. **Fixed CourseServices.DeleteAsync** - Corrected parameter type
3. **Consistent error responses** - Standardized error message format
4. **Comprehensive validation** - Added ownership and existence checks

---

## Next Steps / Recommendations

1. Add pagination for GetQuestions endpoints
2. Implement search/filter functionality
3. Add audit logging for question modifications
4. Consider versioning for question updates
5. Add bulk operations (delete multiple, export/import)

---

## Support

For issues or questions, please refer to the project repository or contact the development team.
