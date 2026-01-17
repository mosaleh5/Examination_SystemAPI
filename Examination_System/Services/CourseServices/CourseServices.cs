using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.Common;
using Examination_System.DTOs.Course;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Services.CourseServices.Repository;
using Examination_System.Services.CourseServices.Validator;
using Examination_System.Specifications.SpecsForEntity;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.CourseServices
{
    public class CourseServices : GenericServices<Course>, ICourseServices
    {
        private readonly IValidator<CreateCourseDto> _createCourseValidator;
        private readonly IValidator<UpdateCourseDto> _updateCourseValidator;
        private readonly ICourseRepository _courseRepository;
        private readonly IValidator<CourseEnrollementDto> _courseEnrollmentDtoValidator;
        private readonly CourseValidator _courseValidator;

        public CourseServices(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CreateCourseDto> createCourseValidator,
            IValidator<UpdateCourseDto> updateCourseValidator,
            ICourseRepository courseRepository,
            IValidator<CourseEnrollementDto> courseEnrollmentDtoValidator,
            CourseValidator courseValidator
            )
            : base(unitOfWork, mapper)
        {
            _createCourseValidator = createCourseValidator;
            _updateCourseValidator = updateCourseValidator;
            _courseRepository = courseRepository;
            _courseEnrollmentDtoValidator = courseEnrollmentDtoValidator;
            _courseValidator = courseValidator; 
        }

        public async Task<Result<IEnumerable<CourseDtoToReturn>>> GetAllForInstructorAsync(Guid instructorId)
        {
            if (instructorId == Guid.Empty)
                return Result<IEnumerable<CourseDtoToReturn>>
                    .Failure(ErrorCode.ValidationError, "Instructor ID is required");

            var courses = _courseRepository.GetByInstructor(instructorId);

            var result = await courses
                .ProjectTo<CourseDtoToReturn>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return result.Any()
                ? Result<IEnumerable<CourseDtoToReturn>>.Success(result)
                : Result<IEnumerable<CourseDtoToReturn>>.Failure(
                    ErrorCode.NotFound,
                    $"No courses found for instructor {instructorId}");
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
            if (courseId == Guid.Empty)
                return Result<CourseDtoToReturn>.Failure(ErrorCode.ValidationError, "Course ID is required");

            if (instructorId == Guid.Empty)
                return Result<CourseDtoToReturn>.Failure(ErrorCode.ValidationError, "Instructor ID is required");

            if (!await _courseRepository.IsInstructorAsync(courseId, instructorId))
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.Forbidden,
                    "You don't have permission to access this course");

            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                return Result<CourseDtoToReturn>.Failure(
                    ErrorCode.NotFound,
                    $"Course with ID {courseId} not found");

            return Result<CourseDtoToReturn>.Success(
                _mapper.Map<CourseDtoToReturn>(course));
        }



        public async Task<Result> EnrollStudentInCourseAsync(CourseEnrollementDto courseEnrollementDto)
        {
            var validationResult = await _courseEnrollmentDtoValidator.ValidateAsync(courseEnrollementDto);
            if (!validationResult.IsValid)
            {
                return Result.ValidaitonFailure(validationResult);
            }

            var ValidateCourseAndGet = await _courseValidator.ValidateEnrollenmentCourse(courseEnrollementDto);
            if(!ValidateCourseAndGet.IsSuccess)
                return ValidateCourseAndGet;
          
            // Enroll student
            var enrollment = new CourseEnrollment
            {
                CourseId = courseEnrollementDto.CourseId,
                StudentId = courseEnrollementDto.StudentId,
                EnrollmentDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CourseEnrollment>().Add(enrollment);
            var rowsAffected = await _unitOfWork.SaveChangesAsync();
            if (rowsAffected < 1)
            {
                return Result.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to enroll student. Database error occurred.");
            }
            return Result.Success();
        }

        public async Task<Result<CourseDtoToReturn>> UpdateAsync(UpdateCourseDto updateCourseDto, Guid userId)
        {
            var validationResult = await _updateCourseValidator.ValidateAsync(updateCourseDto);
            if (!validationResult.IsValid)
            {
                return Result<CourseDtoToReturn>.ValidaitonFailure(validationResult);
            }

            // Check if course exists
            var existingCourse = await _courseRepository.GetByIdAsync(updateCourseDto.ID);

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
           
            var courseToReturn = _mapper.Map<Course, CourseDtoToReturn>(course);
            var result = Result<CourseDtoToReturn>.Success(courseToReturn);
            return result;
        }

        public async Task<Result> DeleteAsync(Guid courseId, Guid instructorId)
        {
          var validationResult = await _courseValidator.ValidateDeleteCourse(courseId, instructorId);
            if (!validationResult.IsSuccess)
                return validationResult;
           var resutl = await _unitOfWork.Repository<Course>().DeleteAsync(courseId);
            if (!resutl)
            {
                return Result.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to delete course. Database error occurred.");
            }
            return Result.Success();
        }

      
    }
}
