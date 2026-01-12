using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.Common;
using Examination_System.DTOs.Course;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.CourseServices
{
    public class CourseServices : GenericServices<Course>, ICourseServices
    {
        private readonly IValidator<CreateCourseDto> _createCourseValidator;
        private readonly IValidator<UpdateCourseDto> _updateCourseValidator;

        public CourseServices(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            IValidator<CreateCourseDto> createCourseValidator,
            IValidator<UpdateCourseDto> updateCourseValidator) 
            : base(unitOfWork, mapper)
        {
            _createCourseValidator = createCourseValidator;
            _updateCourseValidator = updateCourseValidator;
        }

        public async Task<Result<IEnumerable<CourseDtoToReturn>>> GetAllForInstructorAsync(Guid instructorId)
        {
            if (instructorId == null)
            {
                return Result<IEnumerable<CourseDtoToReturn>>.Failure(
                    ErrorCode.ValidationError,
                    "Instructor ID is required");
            }

            var courseSpec = new CourseSpecifications(c => c.InstructorId == instructorId);
            var courses = _unitOfWork.Repository<Course>().GetAllWithSpecificationAsync(courseSpec);
            
            var courseDetailsDto = await courses
                .ProjectTo<CourseDtoToReturn>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!courseDetailsDto.Any())
            {
                return Result<IEnumerable<CourseDtoToReturn>>.Failure(
                    ErrorCode.NotFound,
                    $"No courses found for instructor {instructorId}");
            }

            var result = Result<IEnumerable<CourseDtoToReturn>>.Success(courseDetailsDto);
            return result;
        }

        public async Task<Result<CourseDtoToReturn>> CreateAsync(CreateCourseDto createDto)
        {
            var validationResult = await _createCourseValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                return Result<CourseDtoToReturn>.ValidaitonFailure(validationResult);
            }

            var course = _mapper.Map<CreateCourseDto, Course>(createDto);

            await _unitOfWork.Repository<Course>().Add(course);
            
            var rowsAffected = await _unitOfWork.SaveChangesAsync();
            if (rowsAffected < 1)
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to create course. Database error occurred.");
            }

            var courseToReturn = _mapper.Map<Course, CourseDtoToReturn>(course);
            var result = Result<CourseDtoToReturn>.Success(courseToReturn);
            return result;
        }

        public async Task<Result<CourseDtoToReturn>> GetByIdAsync(Guid courseId, Guid instructorId)
        {
            if (courseId == null)
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.ValidationError,
                    "Valid course ID is required");
            }

            if (instructorId == null)
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.ValidationError,
                    "Instructor ID is required");
            }
            if (! await IsInstructorOfCourseAsync(courseId , instructorId))
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.Forbidden,
                    "You don't have permission to access this course");
            }
            var spec = new CourseSpecifications(c => c.Id == courseId);
            var courseDetails = await _unitOfWork.Repository<Course>()
                .GetByIdWithSpecification(spec)
                .ProjectTo<CourseDtoToReturn>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

            if (courseDetails == null)
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.NotFound,
                    $"Course with ID {courseId} not found");
            }

         

            var result = Result<CourseDtoToReturn>.Success(courseDetails);
            return result;
        }

        public async Task<bool> IsInstructorOfCourseAsync(Guid courseId, Guid instructorId)
        {
          
            var isInstructor = await _unitOfWork.Repository<Course>().GetAll()
                .AnyAsync(c => c.Id == courseId && c.InstructorId == instructorId);
            return isInstructor;
        }

        public async Task<Result> EnrollStudentInCourseAsync(Guid courseId, Guid studentId, Guid instructorId)
        {
            if (courseId == null)
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.ValidationError,
                    "Valid course ID is required");
            }

            if (instructorId == null)
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.ValidationError,
                    "Instructor ID is required");
            }

            if (studentId == null)
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.ValidationError,
                    "Instructor ID is required");
            }
            if (!await IsInstructorOfCourseAsync(courseId, instructorId))
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.Forbidden,
                    "You don't have permission to access this course");
            }

            // Check if course exists
            if (!await _unitOfWork.Repository<Course>().IsExistsAsync(courseId))
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Course with ID {courseId} not found");
            }

            // Check if instructor owns the course
            var isInstructor = await _unitOfWork.Repository<Course>().GetAll()
                .AnyAsync(c => c.Id == courseId && c.InstructorId == instructorId);

            if (!isInstructor)
            {
                return Result.Failure(
                    ErrorCode.Forbidden,
                    "You don't have permission to enroll students in this course");
            }

            if (!await _unitOfWork.Repository<Student>().IsExistsAsync(studentId))
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Student with ID {studentId} not found");
            }

            if (await IsStudentAlreadyEnrolledAsync(courseId, studentId))
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    $"Student {studentId} is already enrolled in course {courseId}");
            }

            var enrollmentCourse = new CourseEnrollment
            {
                CourseId = courseId,
                StudentId = studentId,
                EnrollmentDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CourseEnrollment>().Add(enrollmentCourse);
            
            var rowsAffected = await _unitOfWork.SaveChangesAsync();
            if (rowsAffected < 1)
            {
                return Result.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to enroll student. Database error occurred.");
            }

            return Result.Success();
        }

        private async Task<bool> IsStudentAlreadyEnrolledAsync(Guid courseId, Guid studentId)
        {
            return await _unitOfWork.Repository<CourseEnrollment>().GetAll()
                .AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);
        }

        public async Task<Result<CourseDtoToReturn>> UpdateAsync(UpdateCourseDto updateCourseDto, Guid userId)
        {
            var validationResult = await _updateCourseValidator.ValidateAsync(updateCourseDto);
            if (!validationResult.IsValid)
            {
                return Result<CourseDtoToReturn>.ValidaitonFailure(validationResult);
            }

            // Check if course exists
            var existingCourse = await _unitOfWork.Repository<Course>()
                .GetAll()
                .FirstOrDefaultAsync(c => c.Id == updateCourseDto.ID);

            if (existingCourse == null)
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.NotFound,
                    $"Course with ID {updateCourseDto.ID} not found");
            }

           
            if (existingCourse.InstructorId != userId)
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.Forbidden,
                    "You are not authorized to update this course");
            }

            var course = _mapper.Map<Course>(updateCourseDto);
            await _unitOfWork.Repository<Course>().UpdatePartialAsync(course);
            
            var rowsAffected = await _unitOfWork.SaveChangesAsync();
            if (rowsAffected < 1)
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to update course. Database error occurred.");
            }

            var courseToReturn = _mapper.Map<Course, CourseDtoToReturn>(course);
            var result = Result<CourseDtoToReturn>.Success(courseToReturn);
            return result;
        }

        public async Task<Result> DeleteAsync(Guid courseId, Guid instructorId)
        {
            if (courseId == null)
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.ValidationError,
                    "Valid course ID is required");
            }

            if (instructorId == null)
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.ValidationError,
                    "Instructor ID is required");
            }
            if (!await IsInstructorOfCourseAsync(courseId, instructorId))
            {
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.Forbidden,
                    "You don't have permission to access this course");
            }

            var course = await _unitOfWork.Repository<Course>()
                .GetAll()
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Course with ID {courseId} not found");
            }

            
            if (course.InstructorId != instructorId)
            {
                return Result.Failure(
                    ErrorCode.Forbidden,
                    "You are not authorized to delete this course");
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

            await _unitOfWork.Repository<Course>().DeleteAsync(courseId);
            
            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
            {
                return Result.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to delete course. Database error occurred.");
            }

            return Result.Success();
        }

        public Task<bool> IsInstructorOfCourseAsync(Guid courseId, Guid? instructorId)
        {
            throw new NotImplementedException();
        }
    }
}
