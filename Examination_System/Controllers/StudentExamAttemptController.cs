using AutoMapper;
using Examination_System.Attributes;
using Examination_System.Common;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.Filters;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Services.ExamAttemptServices;
using Examination_System.ViewModels;
using Examination_System.ViewModels.AttemptExam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    [ValidateUserAuthFilterAttribute]
    public class StudentExamAttemptController : BaseController
    {
        private readonly IExamAttemptServices _examAttemptServices;
        private readonly ICurrentUserServices _currentUserServices;


        public StudentExamAttemptController(
            IExamAttemptServices examAttemptServices,
            ICurrentUserServices currentUserServices,
            IMapper mapper) : base(mapper)
        {
            _examAttemptServices = examAttemptServices;
            _currentUserServices = currentUserServices;

        }

        [Authorize(Roles = "Student")]
        [HttpGet("start/{examId:guid}")]
        public async Task<ActionResult<ResponseViewModel<ExamToAttemptDetailedResponseForStudentViewModel>>> StartExam(Guid examId)
        {
            var result = await _examAttemptServices.StartExamAsync(examId, _currentUserServices.UserId);
            return ToResponse<ExamToAttemptDto, ExamToAttemptDetailedResponseForStudentViewModel>(result, "Exam started successfully");
        }


        [Authorize(Roles = "Student")]
        [HttpPost("submit/{attemptId:guid}")]
        public async Task<ActionResult<ResponseViewModel<ExamAttemptResponseForStudentViewModel>>> SubmitExam(
            Guid attemptId,
            [FromBody] IList<SubmitAnswerForStudentViewModel> answers)
        {
            var submitAnswerDtos = _mapper.Map<List<SubmitAnswerDto>>(answers);
            submitAnswerDtos.ForEach(a => a.AttemptId = attemptId);
            var result = await _examAttemptServices.SubmitExamAsync(attemptId, submitAnswerDtos);
            return ToResponse<ExamAttemptDto, ExamAttemptResponseForStudentViewModel>(result, "Exam submitted successfully");

        }


        [Authorize(Roles = "Instructor")]
        [HttpGet("instructor/studentattempts")]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>>> GetAllStudentAttempts()
        {
            var result = await _examAttemptServices.GetStudentAttemptsForInstructorAsync(_currentUserServices.UserId);
            return ToResponse<IEnumerable<ExamAttemptDto>, IEnumerable<ExamAttemptResponseForStudentViewModel>>(result);
        }

        [Authorize(Roles = "Instructor")]
        [HttpGet("instructors/students/{studentId:guid}/exam-attempts")]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>>> GetSpecificStudentAttempts(Guid studentId)
        {
            var result = await _examAttemptServices.GetStudentAttemptsAsync(_currentUserServices.UserId, studentId);
            return ToResponse<ExamAttemptDto, ExamAttemptResponseForStudentViewModel>(result);
        }

        [Authorize(Roles = "Student")]
        [HttpGet("student/my-attempts")]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<ExamAttemptResponseForStudentViewModel>>>> GetMyAttempts()
        {
            var result = await _examAttemptServices.GetStudentAttemptsForStudentAsync(_currentUserServices.UserId);
            return ToResponse<IEnumerable<ExamAttemptDto>, IEnumerable<ExamAttemptResponseForStudentViewModel>>(result);
        }
    }
}