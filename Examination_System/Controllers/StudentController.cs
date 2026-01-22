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
       

        public StudentController(
            IStudentServices studentServices,
            ICurrentUserServices currentUserServices,
            IMapper mapper):base(mapper)    
        {
            _studentServices = studentServices;
            _currentUserServices = currentUserServices;
         
        }

        [HttpGet("my-courses")]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<CourseEnrollmentDto>>>> GetMyCourses()
        {
            var result = await _studentServices.GetEnrolledCoursesAsync(_currentUserServices.UserId);
            return ToResponse<CourseEnrollmentDto, CourseEnrollmentDto>(result);
        }

        [HttpGet("my-exams")]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<ExamAssignmentDto>>>> GetMyExams()
        {    
            var result = await _studentServices.GetAssignedExamsAsync(_currentUserServices.UserId);
            return ToResponse<ExamAssignmentDto, ExamAssignmentDto>(result);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ResponseViewModel<StudentDetailsDto>>> GetMyProfile()
        {
            var result = await _studentServices.GetStudentDetailsAsync(_currentUserServices.UserId);
            return ToResponse<StudentDetailsDto, StudentDetailsDto>(result);
        }
    }
}