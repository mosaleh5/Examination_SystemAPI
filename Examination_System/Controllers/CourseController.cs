using AutoMapper;
using Examination_System.Attributes;
using Examination_System.Common;
using Examination_System.DTOs.Course;
using Examination_System.Filters;
using Examination_System.Models.Enums;
using Examination_System.Services.CourseServices;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Validators.ViewModelValidators.Course;
using Examination_System.ViewModels;
using Examination_System.ViewModels.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    [Authorize(Roles = "Instructor,Admin")]
    [ValidateUserAuthFilterAttribute]
    public class CourseController : BaseController
    {
        private readonly ICourseServices _courseServices;
        private readonly ICurrentUserServices _currentUserServices;

        public CourseController(
            ICourseServices courseServices,
            ICurrentUserServices currentUserServices,
            IMapper mapper) : base(mapper)
        {
            _courseServices = courseServices;
            _currentUserServices = currentUserServices;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<CourseDtoToReturn>>>> GetAllForInstructor()
        {
            var result = await _courseServices.GetAllForInstructorAsync(_currentUserServices.UserId);
            return ToResponse<CourseDtoToReturn, CourseDtoToReturn>(result);
        }

        [HttpGet("{courseId:guid}")]
        public async Task<ActionResult<ResponseViewModel<CourseResponseViewModel>>> GetById(Guid courseId)
        {
            var result = await _courseServices.GetByIdAsync(courseId, _currentUserServices.UserId);
            return ToResponse<CourseDtoToReturn, CourseResponseViewModel>(result);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseViewModel<CourseResponseViewModel>>> Create([FromBody] CreateCourseViewModel createCourseViewModel)
        {
            var createCourseDto = _mapper.Map<CreateCourseDto>(createCourseViewModel);
            createCourseDto.InstructorId = _currentUserServices.UserId;
            var result = await _courseServices.CreateAsync(createCourseDto);
            return ToResponse<CourseDtoToReturn, CourseResponseViewModel>(result, "Course created successfully");
        }

        [HttpPatch("{courseId:guid}/students")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<ResponseViewModel<Result>>> AssignStudentToCourse(Guid courseId, Guid studentId)
        {

            var CourseEnrollmentDto = new CourseEnrollementDto
            {
                CourseId = courseId,
                StudentId = studentId,
                InstructorId = _currentUserServices.UserId
            };
            var result = await _courseServices.EnrollStudentInCourseAsync(CourseEnrollmentDto);
            return ToResponse(result, $"Student with ID {studentId} enrolled in course {courseId} successfully", "An errror occurred ");
        }

        [HttpPut("{courseId:guid}")]
        public async Task<ActionResult<ResponseViewModel<CourseResponseViewModel>>> UpdateCourse(
            Guid courseId,
            [FromBody] UpdateCourseViewModel updateCourseViewModel)
        {

            /* if (courseId != updateCourseViewModel.ID)
                 return BadRequest(ResponseViewModel<CourseResponseViewModel>.Failure(
                     ErrorCode.BadRequest, "Course ID mismatch"));*/

            var updateCourseDto = _mapper.Map<UpdateCourseDto>(updateCourseViewModel);
            var result = await _courseServices.UpdateAsync(updateCourseDto, _currentUserServices.UserId);
            return ToResponse<CourseDtoToReturn, CourseResponseViewModel>(result, "Course updated successfully", "An error occurred while updating the course.");
        }

        [HttpDelete("{courseId:guid}")]
        public async Task<ActionResult<ResponseViewModel<Result>>> DeleteCourse(Guid courseId)
        {
            var result = await _courseServices.DeleteAsync(courseId, _currentUserServices.UserId);
            return ToResponse(result, $"Course with ID {courseId} deleted successfully", "An Error happen when Delete course");
        }


    }
}
