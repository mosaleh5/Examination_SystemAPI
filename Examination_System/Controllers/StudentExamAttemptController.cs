using AutoMapper;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.Models;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Services.ExamAttemptServices;
using Examination_System.ViewModels.AttemptExam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    public class StudentExamAttemptController : BaseController
    {
        private readonly IExamAttemptServices _examAttemptServices;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IMapper _mapper;

        public StudentExamAttemptController(
            IExamAttemptServices examAttemptServices,
            ICurrentUserServices currentUserServices,
            IMapper mapper)
        {
            _examAttemptServices = examAttemptServices;
            _currentUserServices = currentUserServices;
            _mapper = mapper;
        }
        [Authorize(Roles = "Student")]
        [HttpGet("start/{examId}")]
        [ProducesResponseType(typeof(ExamToAttemptDetailedResponseForStudentViewModel), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamToAttemptDetailedResponseForStudentViewModel>> StartExam(int examId)
        {
            var userId = _currentUserServices.UserId;
            if (userId == null || !_currentUserServices.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            if (examId <= 0)
            {
                return BadRequest(new { message = "Invalid exam Id." });
            }

            try
            {
                var exam = await _examAttemptServices.StartExamAsync(examId, userId);
                var examViewModel = _mapper.Map<ExamToAttemptDetailedResponseForStudentViewModel>(exam);
                return Ok(examViewModel);
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
                    new { message = "An error occurred while starting the exam", details = ex.Message });
            }
        }
        [Authorize(Roles = "Student")]

        [HttpPost("submit/{attemptId}")]
        [ProducesResponseType(typeof(ExamAttemptResponseForStudentViewModel), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ExamAttemptResponseForStudentViewModel>> SubmitExam(int attemptId, [FromBody] IList<SubmitAnswerForStudentViewModel> answers)
        {
            var userId = _currentUserServices.UserId;
            if (userId == null || !_currentUserServices.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }
            if (attemptId <= 0)
            {
                return BadRequest(new { message = "Invalid attempt Id." });
            }
            try
            {
                var submitAnswerDtos = _mapper.Map<List<SubmitAnswerDto>>(answers);
                var examAttempt = await _examAttemptServices.SubmitExamAsync(attemptId, submitAnswerDtos);
                var examAttemptViewModel = _mapper.Map<ExamAttemptResponseForStudentViewModel>(examAttempt);
                return Ok(examAttemptViewModel);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while submitting the exam", details = ex.Message });
            }
        }
        [Authorize(Roles = "Instructor")]
        [HttpGet("instructor/studentattempts")]
        [ProducesResponseType(typeof(IEnumerable<ExamAttemptResponseForStudentViewModel>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllStudentAttempts()
        {
            var userId = _currentUserServices.UserId;
            if (userId == null || !_currentUserServices.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            try
            {
                var attempts = await _examAttemptServices.GetStudentAttemptsForInstructorAsync(userId);
                return Ok(attempts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving student attempts", details = ex.Message });
            }
        }

        [Authorize(Roles = "Instructor")]
        [HttpGet("instructor/studentattempts/{studentId}")]
        [ProducesResponseType(typeof(IEnumerable<ExamAttemptResponseForStudentViewModel>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetSpecificStudentAttempts(string studentId)
        {
            var userId = _currentUserServices.UserId;
            if (userId == null || !_currentUserServices.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            try
            {
                var attempts = await _examAttemptServices.GetStudentAttemptsAsync(userId, studentId);
                return Ok(attempts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving student attempts", details = ex.Message });
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("student/myattempts")]
        [ProducesResponseType(typeof(IEnumerable<ExamAttemptResponseForStudentViewModel>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetMyAttempts()
        {
            var userId = _currentUserServices.UserId;
            if (userId == null || !_currentUserServices.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            try
            {
                var attempts = await _examAttemptServices.GetStudentAttemptsForStudentAsync(userId);
                return Ok(attempts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving student attempts", details = ex.Message });
            }
        }

    }
}