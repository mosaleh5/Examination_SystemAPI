using Examination_System.Common;
using Examination_System.DTOs.Course;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Services.CourseServices.Repository;
using Examination_System.Services.StudentService.Repository;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.CourseServices.Validator
{
    public class CourseValidator
    {
        /*        public static Result<T>? ValidateIds<T>(Guid? courseId, Guid? instructorId) where T : class 
                {
                    if (courseId == Guid.Empty)
                        return Result<T>.Failure(ErrorCode.ValidationError, "Course ID is required");

                    if (instructorId == Guid.Empty)
                        return Result<T>.Failure(ErrorCode.ValidationError, "Instructor ID is required");

                    return null;
                }*/
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        public CourseValidator(IUnitOfWork unitOfWork, ICourseRepository courseRepository, IStudentRepository studentRepository)
        {
            _unitOfWork = unitOfWork;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
        }
        public static Result ValidateOwnership(bool isInstructor)
        {
            return isInstructor
                ? Result.Success()
                : Result.Failure(ErrorCode.Forbidden, "You don't have permission to access this course");
        }


        public async Task<Result> ValidateEnrollenmentCourse(CourseEnrollementDto courseEnrollementDto)
        {
            var Course = await _courseRepository.GetByIdAsync(courseEnrollementDto.CourseId);

            if (Course == null)
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Course with ID {courseEnrollementDto.CourseId} not found");
            }

            if (Course.InstructorId != courseEnrollementDto.InstructorId)
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.Forbidden,
                    "You don't have permission to access this course");

            if (await _courseRepository.IsStudentAlreadyEnrolledAsync(courseEnrollementDto.CourseId, courseEnrollementDto.StudentId))
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    $"Student {courseEnrollementDto.StudentId} is already enrolled in course {courseEnrollementDto.CourseId}");
            }
            if (!await _studentRepository.IsExists(courseEnrollementDto.StudentId))
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Student with ID {courseEnrollementDto.StudentId} not found");
            }

            return Result.Success();
        }


        public async  Task<Result> ValidateDeleteCourse(Guid courseId, Guid instructorId) 
        {

            if (courseId == Guid.Empty)
            {
                return Result.Failure(
                    ErrorCode.ValidationError,
                    "Valid course ID is required");
            }

            if (instructorId == Guid.Empty)
            {
                return Result.Failure(
                    ErrorCode.ValidationError,
                    "Instructor ID is required");
            }
            if (!await _courseRepository.IsInstructorOfCourseAsync(courseId, instructorId))
            {
                return Result.Failure(
                    ErrorCode.Forbidden,
                    "You don't have permission to access this course");
            }


            if (!await _courseRepository.ExistsAsync(courseId))
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Course with ID {courseId} not found");
            }

            var hasEnrollments = await _unitOfWork.Repository<CourseEnrollment>()
                .GetAll()
                .AnyAsync(ce => ce.CourseId == courseId);

            if (hasEnrollments)
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    $"Cannot delete course {courseId} because it has enrolled students");
            }

            return Result.Success();    

        }








    }
}
