using AutoMapper;
using Examination_System.Common;
using Examination_System.DTOs.Exam;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Services.CourseServices;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Services.ExamServices;
using Examination_System.Validation;
using Examination_System.ViewModels;
using Examination_System.ViewModels.AttemptExam;
using Examination_System.ViewModels.Exam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    [Authorize(Roles = "Instructor")]
    [ValidateUserAuthentication]
    public class ExamController : BaseController
    {
        private readonly IExamServices _examServices;
        private readonly ICourseServices _courseServices;
        private readonly ICurrentUserServices _currentUserServices;


        public ExamController(
            IExamServices examServices,
            ICourseServices courseServices,
            ICurrentUserServices currentUserServices,
            IMapper mapper ,
            LinkGenerator linkGenerator) : base(mapper ,linkGenerator)
        {
            _examServices = examServices;
            _courseServices = courseServices;
            _currentUserServices = currentUserServices;

        }

        [HttpPost("manual")]
        public async Task<ActionResult<ResponseViewModel<ExamResponseViewModel>>> CreateExam(
            [FromBody] CreateExamViewModel createExamViewModel)
        {
            if (!ModelState.IsValid)
                return ValidationError<ExamResponseViewModel>();

            var createExamDto = _mapper.Map<CreateExamDto>(createExamViewModel);
            createExamDto.InstructorId = _currentUserServices.UserId;
            createExamDto.CreatedAt = DateTime.UtcNow;

            var result = await _examServices.CreateExam(createExamDto);
            var _links = new List<LinkViewModel>()
            {
                SelfLink(),
                CreateLink(nameof(AddQuestionsToExam) , new {examId = result.Data.Id} , "add-Question" , "Post"),
            };
            return ToResponse<ExamToReturnDto, ExamResponseViewModel>(result, "exam created succfully" , "An error Happen", _links);

        }

        [HttpPost("automatic")]
        public async Task<ActionResult<ResponseViewModel<ExamDetailedResponseViewModel>>> CreateAutomaticExam(
            [FromBody] CreateAutomaticExamViewModel createAutomaticExamViewModel)
        {
            if (!ModelState.IsValid)
                return ValidationError<ExamDetailedResponseViewModel>();

            var createAutomaticExamDto = _mapper.Map<CreateAutomaticExamDto>(createAutomaticExamViewModel);
            createAutomaticExamDto.InstructorId = _currentUserServices.UserId;
            createAutomaticExamDto.CreatedAt = DateTime.UtcNow;

            var result = await _examServices.CreateAutomaticExam(createAutomaticExamDto);
            var _links = new List<LinkViewModel>()
            {
                SelfLink(),
                CreateLink(nameof(AddQuestionsToExam) , new {examId = result.Data.Id} , "add-Question" , "Post"),
                CreateLink(nameof(ActivateExam) , new {examId = result.Data.Id} , "Activate-Exam" , "Putch"),
            };
            return ToResponse<ExamToReturnDto, ExamDetailedResponseViewModel>(result,
                "Automatic exam created successfully",
                "An error occurred when creating automatic exam ",
                _links);

        }

        [HttpGet]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<ExamResponseViewModel>>>> GetAllExamsForInstructor()
        {  
            var result = await _examServices.GetAllExamsForInstructor(_currentUserServices.UserId);
            return ToResponse<ExamToReturnDto, ExamResponseViewModel>(result);
        }

        [HttpGet("{examId:guid}")]
        public async Task<ActionResult<ResponseViewModel<ExamDetailedResponseViewModel>>> GetExamsForInstructorById(Guid examId)
        {
            if (CheckId<ExamDetailedResponseViewModel>(examId) is { } badResult)return badResult;

            var GetExamByIdDto = new GetExamByIdDto
            {
                ExamId = examId,
                InstructorId = _currentUserServices.UserId
            };
            var result = await _examServices.GetExamsForInstructorById(GetExamByIdDto);
            return ToResponse<ExamToReturnDto, ExamDetailedResponseViewModel>(result);

        }
        [HttpPut("Activate/{examId:guid}")]
        public async Task<ActionResult<ResponseViewModel<Result>>> ActivateExam(Guid examId)
        {
            if (CheckId<Result>(examId) is { } badResult)return badResult;
            var activateExamDto = new ActivateExamDto
            {
                ExamId = examId,
                InstructorId = _currentUserServices.UserId
            };

            var result = await _examServices.ActivateExamAsync(activateExamDto);
            return ToResponse(result, "Exam activated successfully", "An error occurred when activating exam");
        }

        [HttpPost("manual/{examId:guid}/questions")]
        public async Task<ActionResult<ResponseViewModel<Result>>> AddQuestionsToExam(
            Guid examId,
            [FromBody] List<Guid> questionIds)
        {
            if (!ModelState.IsValid)
                return ValidationError<Result>();

            if (CheckId<Result>(examId) is { } badResult) return badResult;

            var addQuestionsDto = new AddQuestionsToExamDto
            {
                ExamId = examId,
                QuestionIds = questionIds,
                InstructorId = _currentUserServices.UserId
            };

            var result = await _examServices.AddQuestionsToExamAsync(addQuestionsDto);
            return ToResponse(result, $"{questionIds.Count} question(s) added to exam successfully"
                , "An error occurred when adding questions to exam");

        }

        [HttpPut("manual/{examId:guid}/questions")]
        public async Task<ActionResult<ResponseViewModel<Result>>> ReplaceExamQuestions(
            Guid examId,
            [FromBody] List<Guid> questionIds)
        {
            if (!ModelState.IsValid)
                return ValidationError<Result>();

            if (CheckId<Result>(examId) is { } badResult) return badResult;

            var replaceQuestionsDto = new ReplaceExamQuestionsDto
            {
                ExamId = examId,
                QuestionIds = questionIds,
                InstructorId = _currentUserServices.UserId
            };

            var result = await _examServices.ReplaceExamQuestionsAsync(replaceQuestionsDto);
            return ToResponse(result, $"Exam questions replaced successfully with {questionIds.Count} question(s)",
                                "An error occurred when replacing exam questions");
        }

        [HttpDelete("manual/{examId:guid}/questions/{questionId:guid}")]
        public async Task<ActionResult<ResponseViewModel<Result>>> RemoveQuestionFromExam(
            Guid examId,
            Guid questionId)
        {
            if (CheckIds<Result>(examId, questionId) is { } resultCheck) return resultCheck;
            var removeQuestionDto = new RemoveQuestionFromExamDto
            {
                ExamId = examId,
                QuestionId = questionId,
                InstructorId = _currentUserServices.UserId
            };

            var result = await _examServices.RemoveQuestionFromExamAsync(removeQuestionDto);
            return ToResponse(result, "Question removed from exam successfully", "An error occurred when removing question from exam");
        }

        [HttpPost("{examId:guid}/students")]
        public async Task<ActionResult<ResponseViewModel<Result>>> AssignStudentToExam(
            Guid examId,
            [FromBody] Guid studentId)
        {
            if (!ModelState.IsValid)
                return ValidationError<Result>();

            if (CheckId<Result>(examId) is { } badResult) return badResult;

            var assignStudentDto = new AssignStudentToExamDto
            {
                ExamId = examId,
                StudentId = studentId,
                InstructorId = _currentUserServices.UserId
            };

            var result = await _examServices.EnrollStudentToExamAsync(assignStudentDto);
            return ToResponse(result, "Student enrolled in exam successfully", "An error occurred when enrolling student to exam");
        }

    }
}
