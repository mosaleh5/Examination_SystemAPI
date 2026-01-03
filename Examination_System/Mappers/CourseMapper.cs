using Examination_System.DTOs.Course;
using Examination_System.Models;
using Microsoft.IdentityModel.Tokens;

namespace Examination_System.Mappers
{
    public static class CourseMapper
    {
        public static CourseDto ToDto(Course course)
        {
            if (course == null) return null;

            return new CourseDto
            {
                ID = course.Id,
                Name = course.Name,
                Description = course.Description,
                Hours = course.Hours,
                InstructorId = course.InstructorId,
                InstructorName = course.Instructor != null
                    ? $"{course.Instructor.User.FirstName} {course.Instructor.User.LastName}"
                    : string.Empty,
                CreatedAt = course.CreatedAt
            };
        }

        public static CourseDetailsDto ToDetailsDto(Course course)
        {
            if (course == null) return null;

            return new CourseDetailsDto
            {
                //ID = course.Id,
                Name = course.Name,
                Description = course.Description,
                Hours = course.Hours,
                CreatedAt = course.CreatedAt,
                InstructorId = course.InstructorId,
                InstructorName = course.Instructor != null
                    ? $"{course.Instructor.User.FirstName} {course.Instructor.User.LastName}"
                    : string.Empty,
                InstructorEmail = course.Instructor?.User?.Email ?? string.Empty,
                EnrolledStudentsCount = course.CourseEnrollments?.Count ?? 0,
                ExamsCount = course.Exams?.Count ?? 0,
                Exams = course.Exams?.Select(e => new CourseExamDto
                {
                    ID = e.Id,
                    Title = e.Title,
                    Date = e.Date,
                    DurationMinutes = e.DurationInMinutes,
                    ExamType = e.ExamType.ToString()
                }).ToList() ?? new List<CourseExamDto>(),
                EnrolledStudents = course.CourseEnrollments?.Select(ce => new EnrolledStudentDto
                {
                    StudentId = ce.StudentId,
                    StudentName = ce.Student?.User != null
                        ? $"{ce.Student.User.FirstName} {ce.Student.User.LastName}"
                        : string.Empty,
                    Major = ce.Student?.Major ?? string.Empty,
                    EnrollmentDate = ce.EnrollmentDate
                }).ToList() ?? new List<EnrolledStudentDto>()
            };
        }

        public static Course ToEntity(CreateCourseDto dto)
        {
            if (dto == null) return null;

            return new Course
            {
                Name = dto.Name,
                Description = dto.Description,
                Hours = dto.Hours,
                InstructorId = dto.InstructorId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
        }

        public static void UpdateEntity(Course course, UpdateCourseDto dto)
        {
            if (course == null || dto == null) return;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                course.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                course.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.Hours))
                course.Hours = dto.Hours;

            if (dto.InstructorId.IsNullOrEmpty())
                course.InstructorId = dto.InstructorId;
        }

        public static IEnumerable<CourseDto> ToDtoList(IEnumerable<Course> courses)
        {
            return courses?.Select(ToDto).Where(dto => dto != null) ?? Enumerable.Empty<CourseDto>();
        }
    }
}
