using AutoMapper;
using Examination_System.Common;
using Examination_System.DTOs.Exam;
using Examination_System.Models.Enums;
using Examination_System.Services.CourseServices;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Services.ExamServices;
using Examination_System.ViewModels;
using Examination_System.ViewModels.AttemptExam;
using Examination_System.ViewModels.Exam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class ExamController : BaseController
    {
        private readonly IExamServices _examServices;
        private readonly ICourseServices _courseServices;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IMapper _mapper;

        public ExamController(
            IExamServices examServices,
            ICourseServices courseServices,
            ICurrentUserServices currentUserServices,
            IMapper mapper)
        {
            _examServices = examServices;
            _courseServices = courseServices;
            _currentUserServices = currentUserServices;
            _mapper = mapper;
        }

        [HttpPost("manual")]
        [ProducesResponseType(typeof(ResponseViewModel<ExamResponseViewModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel<ExamResponseViewModel>>> CreateExam(
            [FromBody] CreateExamViewModel createExamViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseViewModel<ExamResponseViewModel>.Failure(
                    ErrorCode.ValidationError,
                    GetValidationErrors()));

            var createExamDto = _mapper.Map<CreateExamDto>(createExamViewModel);
            createExamDto.InstructorId = _currentUserServices.UserId;
            createExamDto.CreatedAt = DateTime.UtcNow;

            var result = await _examServices.CreateExam(createExamDto);

            if (!result.IsSuccess)
                return BadRequest(ResponseViewModel<ExamResponseViewModel>.Failure(
                    result.Error,
                    "An error occurred when creating exam\n" + result.ErrorMessage));

            var examViewModel = _mapper.Map<ExamResponseViewModel>(result.Data);
            return CreatedAtAction(
                nameof(GetAllExamsForInstructor),
                ResponseViewModel<ExamResponseViewModel>.Success(examViewModel, "Exam created successfully"));
        }

        [HttpPost("automatic")]
        [ProducesResponseType(typeof(ResponseViewModel<ExamDetailedResponseViewModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel<ExamDetailedResponseViewModel>>> CreateAutomaticExam(
            [FromBody] CreateAutomaticExamViewModel createAutomaticExamViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseViewModel<ExamDetailedResponseViewModel>.Failure(
                    ErrorCode.ValidationError,
                    GetValidationErrors()));

            var createAutomaticExamDto = _mapper.Map<CreateAutomaticExamDto>(createAutomaticExamViewModel);
            createAutomaticExamDto.InstructorId = _currentUserServices.UserId;
            createAutomaticExamDto.CreatedAt = DateTime.UtcNow;

            var result = await _examServices.CreateAutomaticExam(createAutomaticExamDto);

            if (!result.IsSuccess)
                return BadRequest(ResponseViewModel<ExamDetailedResponseViewModel>.Failure(
                    result.Error,
                    "An error occurred when creating automatic exam\n" + result.ErrorMessage));

            var createdExamViewModel = _mapper.Map<ExamDetailedResponseViewModel>(result.Data);
            return CreatedAtAction(
                nameof(GetAllExamsForInstructor),
                ResponseViewModel<ExamDetailedResponseViewModel>.Success(createdExamViewModel, "Automatic exam created successfully"));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseViewModel<IEnumerable<ExamResponseViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<ExamResponseViewModel>>>> GetAllExamsForInstructor()
        {
            if (_currentUserServices.UserId == null)
                return Unauthorized(ResponseViewModel<IEnumerable<ExamResponseViewModel>>.Failure(
                    ErrorCode.Unauthorized,
                    "User not authenticated"));

            var result = await _examServices.GetAllExamsForInstructor(_currentUserServices.UserId);

            if (!result.IsSuccess)
                return BadRequest(ResponseViewModel<IEnumerable<ExamResponseViewModel>>.Failure(
                    result.Error,
                    result.ErrorMessage));

            var examViewModels = _mapper.Map<IEnumerable<ExamResponseViewModel>>(result.Data);
            return Ok(ResponseViewModel<IEnumerable<ExamResponseViewModel>>.Success(examViewModels));
        }

        [HttpGet("{examId}")]
        [ProducesResponseType(typeof(ResponseViewModel<IEnumerable<ExamResponseViewModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel<ExamDetailedResponseViewModel>>> GetExamsForInstructorById(Guid examId)
        {
            if (_currentUserServices.UserId == null)
                return Unauthorized(ResponseViewModel<ExamDetailedResponseViewModel>.Failure(
                    ErrorCode.Unauthorized,
                    "User not authenticated"));
            var GetExamByIdDto = new GetExamByIdDto
            {
                ExamId = examId,
                InstructorId = _currentUserServices.UserId
            };
            var result = await _examServices.GetExamsForInstructorById(GetExamByIdDto);

            if (!result.IsSuccess)
                return BadRequest(ResponseViewModel<ExamDetailedResponseViewModel>.Failure(
                    result.Error,
                    result.ErrorMessage));

            var examViewModels = _mapper.Map<ExamDetailedResponseViewModel>(result.Data);
            return Ok(ResponseViewModel<ExamDetailedResponseViewModel>.Success(examViewModels));
        }
        [HttpPut("Activate/{examId}")]
        [ProducesResponseType(typeof(ResponseViewModel<Result>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel<Result>>> ActivateExam(Guid examId)
        {
            var activateExamDto = new ActivateExamDto
            {
                ExamId = examId,
                InstructorId = _currentUserServices.UserId
            };

            var result = await _examServices.ActivateExamAsync(activateExamDto);

            return result.IsSuccess
                ? Ok(ResponseViewModel<Result>.Success(result, "Exam activated successfully"))
                : BadRequest(ResponseViewModel<Result>.Failure(
                    result.Error,
                    "An error occurred when activating exam\n" + result.ErrorMessage));
        }

        [HttpPost("manual/{examId}/questions")]
        [ProducesResponseType(typeof(ResponseViewModel<Result>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel<Result>>> AddQuestionsToExam(
            Guid examId,
            [FromBody] List<Guid> questionIds)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseViewModel<Result>.Failure(
                    ErrorCode.ValidationError,
                    GetValidationErrors()));        

            var addQuestionsDto = new AddQuestionsToExamDto
            {
                ExamId = examId,
                QuestionIds = questionIds,
                InstructorId = _currentUserServices.UserId
            };

            var result = await _examServices.AddQuestionsToExamAsync(addQuestionsDto);

            return result.IsSuccess
                ? Ok(ResponseViewModel<Result>.Success(result, $"{questionIds.Count} question(s) added to exam successfully"))
                : BadRequest(ResponseViewModel<Result>.Failure(
                    result.Error,
                    "An error occurred when adding questions to exam\n" + result.ErrorMessage));
        }

        [HttpPut("manual/{examId}/questions")]
        [ProducesResponseType(typeof(ResponseViewModel<Result>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel<Result>>> ReplaceExamQuestions(
            Guid examId,
            [FromBody] List<Guid> questionIds)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseViewModel<Result>.Failure(
                    ErrorCode.ValidationError,
                    GetValidationErrors()));

            var replaceQuestionsDto = new ReplaceExamQuestionsDto
            {
                ExamId = examId,
                QuestionIds = questionIds,
                InstructorId = _currentUserServices.UserId
            };

            var result = await _examServices.ReplaceExamQuestionsAsync(replaceQuestionsDto);

            return result.IsSuccess
                ? Ok(ResponseViewModel<Result>.Success(result, $"Exam questions replaced successfully with {questionIds.Count} question(s)"))
                : BadRequest(ResponseViewModel<Result>.Failure(
                    result.Error,
                    "An error occurred when replacing exam questions\n" + result.ErrorMessage));
        }

        [HttpDelete("manual/{examId}/questions/{questionId}")]
        [ProducesResponseType(typeof(ResponseViewModel<Result>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel<Result>>> RemoveQuestionFromExam(
            Guid examId,
            Guid questionId)
        {
            var removeQuestionDto = new RemoveQuestionFromExamDto
            {
                ExamId = examId,
                QuestionId = questionId,
                InstructorId = _currentUserServices.UserId
            };

            var result = await _examServices.RemoveQuestionFromExamAsync(removeQuestionDto);

            return result.IsSuccess
                ? Ok(ResponseViewModel<Result>.Success(result, "Question removed from exam successfully"))
                : BadRequest(ResponseViewModel<Result>.Failure(
                    result.Error,
                    "An error occurred when removing question from exam\n" + result.ErrorMessage));
        }

        [HttpPost("{examId}/students")]
        [ProducesResponseType(typeof(ResponseViewModel<Result>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel<Result>>> AssignStudentToExam(
            Guid examId,
            [FromBody] Guid studentId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseViewModel<Result>.Failure(
                    ErrorCode.ValidationError,
                    GetValidationErrors()));

            var assignStudentDto = new AssignStudentToExamDto
            {
                ExamId = examId,
                StudentId = studentId,
                InstructorId = _currentUserServices.UserId
            };

            var result = await _examServices.EnrollStudentToExamAsync(assignStudentDto);

            return result.IsSuccess
                ? Ok(ResponseViewModel<Result>.Success(result, "Student enrolled in exam successfully"))
                : BadRequest(ResponseViewModel<Result>.Failure(
                    result.Error,
                    "An error occurred when enrolling student to exam\n" + result.ErrorMessage));
        }
        
    }
}
