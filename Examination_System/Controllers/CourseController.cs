using AutoMapper;
using Examination_System.DTOs.Course;
using Examination_System.Models;
using Examination_System.Services.CourseServices;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Services.StudentService;
using Examination_System.Specifications.SpecsForEntity;
using Examination_System.Validation;
using Examination_System.ViewModels.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Examination_System.Controllers
{
   
    [Authorize(Roles = "Instructor , Admin")]

    public class CourseController : BaseController
    {
        private readonly ICourseServices _courseServices;
        private readonly IStudentServices _studentServices;

        private readonly ICurrentUserServices _currentUserServices;
        private readonly IMapper _mapper;

        public CourseController(ICourseServices courseServices,
            ICurrentUserServices CurrentUserServices,
            IMapper Mapper)
        {
            _courseServices = courseServices;
            _currentUserServices = CurrentUserServices;
            _mapper = Mapper;
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CourseResponseViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CourseResponseViewModel>>> GetAllForInstructor()
        {
                 
            var coursesResult = await _courseServices.GetAllForInstructorAsync(_currentUserServices.UserId);
            var courses = _mapper.Map<IEnumerable<CourseResponseViewModel>>(coursesResult);

            return Ok(courses);
        }
        [HttpPost]
        [ProducesResponseType(typeof(CourseResponseViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<ActionResult<CourseResponseViewModel>> Create([FromBody] CreateCourseViewModel createCourseViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           
         

            var CreateCourseDto = _mapper.Map<CreateCourseViewModel, CreateCourseDto>(createCourseViewModel);

            CreateCourseDto.InstructorId = _currentUserServices.UserId;
            //createDto.InstructorName = userName;
            try
            {
                var coursesResult = await _courseServices.CreateAsync(CreateCourseDto);
                var courses = _mapper.Map<CourseResponseViewModel>(coursesResult);
                return courses;
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while creating the course", details = ex.Message });
            }
        }
        [HttpPatch("{courseId}/students")]
        [Authorize(Roles = "Instructor,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        

        public async Task<ActionResult> EnrollStudentInCourse(int courseId, string studentId)
        {

         
           
            if (!await _courseServices.IsInstructorOfCourseAsync(courseId, _currentUserServices.UserId))
            {
                return Unauthorized(new { Message = "You are not Allowed to assign student to this course" });
            }
        //if(!EnrollStudentInCourseValidation())
            try
            {
                await _courseServices.EnrollStudentInCourseAsync(courseId, studentId);
                return Ok(new { message = $"Student with ID {studentId} enrolled in course {courseId} successfully." });

            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while enrolling the student in the course", details = ex.Message });
            }
        }
        private async Task<ActionResult> EnrollStudentInCourseValidation(int courseId, string studentId)
        {
          
            if (string.IsNullOrEmpty(studentId))
            {
                return BadRequest(new { message = "StudentId cannot be null or empty." });
            }
            if (courseId <= 0)
            {
                return BadRequest(new { message = "Invalid courseId." });
            }
            return Ok();
        }
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<CourseResponseViewModel>> UpdateCourse([FromBody] UpdateCourseViewModel updateCourseViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

       
            var updateCourseDto = _mapper.Map<UpdateCourseDto>(updateCourseViewModel);
            try
            {
                var courseExists = await _courseServices.IsInstructorOfCourseAsync(updateCourseDto.ID, _currentUserServices.UserId);
                var courseResult = await _courseServices.UpdateAsync(updateCourseDto, _currentUserServices.UserId);
                var course = _mapper.Map<CourseResponseViewModel>(courseResult);
                return Ok(course);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while updating the course", details = ex.Message });
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteCourse(int courseId)
        {
           
            try
            {
                var course = await _courseServices.IsInstructorOfCourseAsync(courseId , _currentUserServices.UserId);
                if (!course)
                {
                    return NotFound(new { message = $"Course with ID {courseId} not found or you are not authorized to delete it." });
                }
                await _courseServices.DeleteAsync(courseId);
                return Ok(new { message = $"Course with ID {courseId} deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while deleting the course", details = ex.Message });
            }
        }
    }
}
