using AutoMapper;
using Examination_System.Common;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.Models.Enums;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Services.ExamAttemptServices;
using Examination_System.Validation;
using Examination_System.ViewModels;
using Examination_System.ViewModels.AttemptExam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    [ValidateUserAuthentication]
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
        [ProducesResponseType(typeof(ResponseViewModel<ExamToAttemptDetailedResponseForStudentViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseViewModel<ExamToAttemptDetailedResponseForStudentViewModel>>> StartExam(Guid examId)
        {
            if (_currentUserServices.UserId == null)
            {
                return Unauthorized(ResponseViewModel<ExamToAttemptDetailedResponseForStudentViewModel>.Failure(
                    ErrorCode.Unauthorized,
                    "User not authenticated"));
            }

            var result = await _examAttemptServices.StartExamAsync(examId, _currentUserServices.UserId);

            if (!result.IsSuccess)
            {
                return BadRequest(ResponseViewModel<ExamToAttemptDetailedResponseForStudentViewModel>.Failure(
                    result.Error,
                    result.ErrorMessage));
            }

            var examViewModel = _mapper.Map<ExamToAttemptDto, ExamToAttemptDetailedResponseForStudentViewModel>(result.Data);
            return Ok(ResponseViewModel<ExamToAttemptDetailedResponseForStudentViewModel>.Success(
                examViewModel,
                "Exam started successfully"));
        }

        [Authorize(Roles = "Student")]
        [HttpPost("submit/{attemptId}")]
        [ProducesResponseType(typeof(ResponseViewModel<ExamAttemptResponseForStudentViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseViewModel<ExamAttemptResponseForStudentViewModel>>> SubmitExam(
            Guid attemptId,
            [FromBody] IList<SubmitAnswerForStudentViewModel> answers)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseViewModel<ExamAttemptResponseForStudentViewModel>.Failure(
                    ErrorCode.ValidationError,
                    GetValidationErrors()));
            }

            var submitAnswerDtos = _mapper.Map<List<SubmitAnswerDto>>(answers);
            submitAnswerDtos.ForEach(a => a.AttemptId = attemptId);

            var result = await _examAttemptServices.SubmitExamAsync(attemptId, submitAnswerDtos);

            if (!result.IsSuccess)
            {
                return BadRequest(ResponseViewModel<ExamAttemptResponseForStudentViewModel>.Failure(
                    result.Error,
                    "An error occurred when submitting exam\n" + result.ErrorMessage));
            }

            var examAttemptViewModel = _mapper.Map<ExamAttemptResponseForStudentViewModel>(result.Data);
            return Ok(ResponseViewModel<ExamAttemptResponseForStudentViewModel>.Success(
                examAttemptViewModel,
                "Exam submitted successfully"));
        }

        [Authorize(Roles = "Instructor")]
        [HttpGet("instructor/studentattempts")]
        [ProducesResponseType(typeof(ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>>> GetAllStudentAttempts()
        {
            if (_currentUserServices.UserId == null)
            {
                return Unauthorized(ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>.Failure(
                    ErrorCode.Unauthorized,
                    "User not authenticated"));
            }

            var result = await _examAttemptServices.GetStudentAttemptsForInstructorAsync(_currentUserServices.UserId);

            if (!result.IsSuccess)
            {
                return BadRequest(ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>.Failure(
                    result.Error,
                    result.ErrorMessage));
            }

            var examAttemptViewModels = _mapper.Map<IEnumerable<ExamAttemptResponseForStudentViewModel>>(result.Data);

            return Ok(ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>.Success(
                examAttemptViewModels));
        }

        [Authorize(Roles = "Instructor")]
        [HttpGet("{studentId}")]
        [ProducesResponseType(typeof(ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>>> GetSpecificStudentAttempts(Guid studentId)
        {
            if (_currentUserServices.UserId == null)
            {
                return Unauthorized(ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>.Failure(
                    ErrorCode.Unauthorized,
                    "User not authenticated"));
            }

            if (studentId == Guid.Empty)
            {
                return BadRequest(ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>.Failure(
                    ErrorCode.ValidationError,
                    "Student ID is required"));
            }

            var result = await _examAttemptServices.GetStudentAttemptsAsync(_currentUserServices.UserId, studentId);

            if (!result.IsSuccess)
            {
                return BadRequest(ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>.Failure(
                    result.Error,
                    result.ErrorMessage));
            }

            var examAttemptViewModels = _mapper.Map<IEnumerable<ExamAttemptResponseForStudentViewModel>>(result.Data);
            return Ok(ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>.Success(
                examAttemptViewModels));
        }

        [Authorize(Roles = "Student")]
        [HttpGet("student/myattempts")]
        [ProducesResponseType(typeof(ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>>> GetMyAttempts()
        {
            if (_currentUserServices.UserId == null)
            {
                return Unauthorized(ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>.Failure(
                    ErrorCode.Unauthorized,
                    "User not authenticated"));
            }

            var result = await _examAttemptServices.GetStudentAttemptsForStudentAsync(_currentUserServices.UserId);

            if (!result.IsSuccess)
            {
                return BadRequest(ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>.Failure(
                    result.Error,
                    result.ErrorMessage));
            }

            var examAttemptViewModels = _mapper.Map<IEnumerable<ExamAttemptResponseForStudentViewModel>>(result.Data);
            return Ok(ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>.Success(
                examAttemptViewModels));
        }
    }
}