using AutoMapper;
using Examination_System.Common;
using Examination_System.DTOs.Course;
using Examination_System.Models.Enums;
using Examination_System.Services.CourseServices;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Validation;
using Examination_System.Validators.ViewModelValidators.Course;
using Examination_System.ViewModels;
using Examination_System.ViewModels.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    [Authorize(Roles = "Instructor , Admin")]
    [ValidateUserAuthentication]
    public class CourseController : BaseController
    {
        private readonly ICourseServices _courseServices;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IMapper _mapper;

        public CourseController(
            ICourseServices courseServices,
            ICurrentUserServices currentUserServices,
            IMapper mapper)
        {
            _courseServices = courseServices;
            _currentUserServices = currentUserServices;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseViewModel<IEnumerable<CourseDtoToReturn>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<CourseDtoToReturn>>>> GetAllForInstructor()
        {
            var result = await _courseServices.GetAllForInstructorAsync(_currentUserServices.UserId);

            return result.IsSuccess 
                ? Ok(ResponseViewModel<IEnumerable<CourseDtoToReturn>>.Success(result.Data))
                : BadRequest(ResponseViewModel<IEnumerable<CourseDtoToReturn>>.Failure(result.Error, result.ErrorMessage));
        }

        [HttpGet("{courseId}")]
        [ProducesResponseType(typeof(ResponseViewModel<CourseDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseViewModel<CourseResponseViewModel>>> GetById(Guid courseId)
        {
            if (_currentUserServices.UserId == null)
                return Unauthorized(ResponseViewModel<CourseResponseViewModel>.Failure(
                    ErrorCode.Unauthorized,
                    "User not authenticated"));

            var result = await _courseServices.GetByIdAsync(courseId, _currentUserServices.UserId);
            var Data = _mapper.Map<CourseResponseViewModel>(result.Data);
            
            return result.IsSuccess 
                ? Ok(ResponseViewModel<CourseResponseViewModel>.Success(Data))
                : BadRequest(ResponseViewModel<CourseResponseViewModel>.Failure(result.Error, result.ErrorMessage));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseViewModel<CourseResponseViewModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseViewModel<CourseResponseViewModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel<CourseResponseViewModel>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseViewModel<CourseResponseViewModel>>> Create([FromBody] CreateCourseViewModel createCourseViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseViewModel<CourseResponseViewModel>.Failure(
                    ErrorCode.ValidationError,
                    GetValidationErrors()));

            var createCourseDto = _mapper.Map<CreateCourseDto>(createCourseViewModel);
            createCourseDto.InstructorId = _currentUserServices.UserId;

            var result = await _courseServices.CreateAsync(createCourseDto);
            var Data = _mapper.Map<CourseResponseViewModel>(result.Data);


            return result.IsSuccess 
                ? CreatedAtAction(
                    nameof(GetById),
                    new { courseId = result.Data.Id },
                    ResponseViewModel<CourseDtoToReturn>.Success(result.Data, "Course created successfully"))
                : BadRequest(ResponseViewModel<CourseDtoToReturn>.Failure(
                    result.Error, 
                    "An error occurred when creating course\n" + result.ErrorMessage));
        }

        [HttpPatch("{courseId}/students")]
        [Authorize(Roles = "Instructor,Admin")]
        [ProducesResponseType(typeof(ResponseViewModel<Result>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseViewModel<Result>>> AssignStudentToCourse(Guid courseId, Guid studentId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseViewModel<Result>.Failure(
                    ErrorCode.ValidationError,
                    GetValidationErrors()));

            var request = new EnrollStudentRequest { CourseId = courseId, StudentId = studentId };
            
            // Check if instructor has permission
            var authResult = await _courseServices.IsInstructorOfCourseAsync(courseId, _currentUserServices.UserId);
            if (!authResult)
            {
                return Unauthorized(ResponseViewModel<Result>.Failure(
                    ErrorCode.Unauthorized,
                    "You are not allowed to assign students to this course"));
            }

            var result = await _courseServices.EnrollStudentInCourseAsync(courseId, studentId, _currentUserServices.UserId);

            return result.IsSuccess 
                ? Ok(ResponseViewModel<Result>.Success(result, $"Student with ID {studentId} enrolled in course {courseId} successfully"))
                : BadRequest(ResponseViewModel<Result>.Failure(
                    result.Error, 
                    "An error occurred while enrolling the student\n" + result.ErrorMessage));
        }

        [HttpPut("{courseId}")]
        [ProducesResponseType(typeof(ResponseViewModel<CourseResponseViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<CourseResponseViewModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel<CourseResponseViewModel>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseViewModel<CourseResponseViewModel>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseViewModel<CourseResponseViewModel>>> UpdateCourse(
            Guid courseId,
            [FromBody] UpdateCourseViewModel updateCourseViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseViewModel<CourseResponseViewModel>.Failure(
                    ErrorCode.ValidationError,
                    GetValidationErrors()));

            if (courseId != updateCourseViewModel.ID)
                return BadRequest(ResponseViewModel<CourseResponseViewModel>.Failure(
                    ErrorCode.BadRequest,
                    "Course ID mismatch"));

            var updateCourseDto = _mapper.Map<UpdateCourseDto>(updateCourseViewModel);

            var result = await _courseServices.UpdateAsync(updateCourseDto, _currentUserServices.UserId);
            var Data = _mapper.Map<CourseResponseViewModel>(result.Data);

            return result.IsSuccess 
                ? Ok(ResponseViewModel<CourseResponseViewModel>.Success(Data, "Course updated successfully"))
                : BadRequest(ResponseViewModel<CourseResponseViewModel>.Failure(result.Error, result.ErrorMessage));
        }

        [HttpDelete("{courseId}")]
        [ProducesResponseType(typeof(ResponseViewModel<Result>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseViewModel<Result>>> DeleteCourse(Guid courseId)
        {
            if (courseId == null)
                return BadRequest(ResponseViewModel<Result>.Failure(
                    ErrorCode.BadRequest,
                    "Invalid course ID"));

            var result = await _courseServices.DeleteAsync(courseId, _currentUserServices.UserId);

            return result.IsSuccess 
                ? Ok(ResponseViewModel<Result>.Success(result, $"Course with ID {courseId} deleted successfully"))
                : BadRequest(ResponseViewModel<Result>.Failure(
                    result.Error, 
                    "An error occurred when deleting course\n" + result.ErrorMessage));
        }
    }
}
