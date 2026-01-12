using AutoMapper;
using Examination_System.Common;
using Examination_System.DTOs.Student;
using Examination_System.Models.Enums;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Services.StudentService;
using Examination_System.Validation;
using Examination_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    [Authorize(Roles = "Student")]
    [ValidateUserAuthentication]
    public class StudentController : BaseController
    {
        private readonly IStudentServices _studentServices;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IMapper _mapper;

        public StudentController(
            IStudentServices studentServices,
            ICurrentUserServices currentUserServices,
            IMapper mapper)
        {
            _studentServices = studentServices;
            _currentUserServices = currentUserServices;
            _mapper = mapper;
        }

        [HttpGet("my-courses")]
        [ProducesResponseType(typeof(ResponseViewModel<IEnumerable<CourseEnrollmentDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<CourseEnrollmentDto>>>> GetMyCourses()
        {
            if (_currentUserServices.UserId == null)
                return Unauthorized(ResponseViewModel<IEnumerable<CourseEnrollmentDto>>.Failure(
                    ErrorCode.Unauthorized,
                    "User not authenticated"));

            var result = await _studentServices.GetEnrolledCoursesAsync(_currentUserServices.UserId);

            return result.IsSuccess
                ? Ok(ResponseViewModel<IEnumerable<CourseEnrollmentDto>>.Success(result.Data))
                : BadRequest(ResponseViewModel<IEnumerable<CourseEnrollmentDto>>.Failure(
                    result.Error,
                    result.ErrorMessage));
        }

        [HttpGet("my-exams")]
        [ProducesResponseType(typeof(ResponseViewModel<IEnumerable<ExamAssignmentDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<ExamAssignmentDto>>>> GetMyExams()
        {
            if (_currentUserServices.UserId == null)
                return Unauthorized(ResponseViewModel<IEnumerable<ExamAssignmentDto>>.Failure(
                    ErrorCode.Unauthorized,
                    "User not authenticated"));

            var result = await _studentServices.GetAssignedExamsAsync(_currentUserServices.UserId);

            return result.IsSuccess
                ? Ok(ResponseViewModel<IEnumerable<ExamAssignmentDto>>.Success(result.Data))
                : BadRequest(ResponseViewModel<IEnumerable<ExamAssignmentDto>>.Failure(
                    result.Error,
                    result.ErrorMessage));
            /*      }

                  [HttpGet("profile")]
                  [ProducesResponseType(typeof(ResponseViewModel<StudentDetailsDto>), StatusCodes.Status200OK)]
                  public async Task<ActionResult<ResponseViewModel<StudentDetailsDto>>> GetMyProfile()
                  {
                      // _currentUserServices.UserId is now a Guid string
                      if (!Guid.TryParse(_currentUserServices.UserId, out var studentId))
                      {
                          return BadRequest(ResponseViewModel<StudentDetailsDto>.Failure(
                              ErrorCode.ValidationError,
                              "Invalid student ID"));
                      }

                      var result = await _studentServices.GetStudentDetailsAsync(studentId);

                      return result.IsSuccess
                          ? Ok(ResponseViewModel<StudentDetailsDto>.Success(result.Data))
                          : BadRequest(ResponseViewModel<StudentDetailsDto>.Failure(
                              result.Error,
                              result.ErrorMessage));
                  }*/
        }
    }
}