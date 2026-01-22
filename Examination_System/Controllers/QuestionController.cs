using AutoMapper;
using Examination_System.Common;
using Examination_System.DTOs.Question;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Services.QuestionServices;
using Examination_System.Validation;
using Examination_System.ViewModels;
using Examination_System.ViewModels.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    /// <summary>
    /// Controller for managing questions
    /// </summary>
    [Authorize(Roles = "Instructor,Admin")]
    [ValidateUserAuthentication]
    public class QuestionController : BaseController
    {
        private readonly IQuestionServices _questionServices;
        private readonly ICurrentUserServices _currentUser;

        public QuestionController(
            IQuestionServices questionServices,
            IMapper mapper,
            ICurrentUserServices currentUser)
            : base(mapper)
        {
            _questionServices = questionServices;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Gets a question by ID
        /// </summary>
        [HttpGet("{questionId:guid}")]
        [ProducesResponseType(typeof(ResponseViewModel<QuestionToReturnViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<QuestionToReturnViewModel>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseViewModel<QuestionToReturnViewModel>>> GetById(Guid questionId)
        {
            var result = await _questionServices.GetByIdAsync(questionId, _currentUser.UserId);
            return ToResponse<QuestionToReturnDto, QuestionToReturnViewModel>(result);
        }

        /// <summary>
        /// Gets all questions for the current instructor
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseViewModel<IEnumerable<QuestionToReturnViewModel>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<QuestionToReturnViewModel>>>> GetAll()
        {
            var result = await _questionServices.GetByInstructorAsync(_currentUser.UserId);
            return ToResponse<QuestionToReturnDto, QuestionToReturnViewModel>(result);
        }

        /// <summary>
        /// Gets all questions for the current instructor in a specific course
        /// </summary>
        [HttpGet("course/{courseId:guid}")]
        [ProducesResponseType(typeof(ResponseViewModel<IEnumerable<QuestionToReturnViewModel>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseViewModel<IEnumerable<QuestionToReturnViewModel>>>> GetByCourse(Guid courseId)
        {
            var result = await _questionServices.GetByInstructorAndCourseAsync(_currentUser.UserId, courseId);
            return ToResponse<QuestionToReturnDto, QuestionToReturnViewModel>(result);
        }

        /// <summary>
        /// Creates a new question
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseViewModel<QuestionToReturnViewModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseViewModel<QuestionToReturnViewModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseViewModel<QuestionToReturnViewModel>>> Create(
            [FromBody] CreateQuestionViewModel model)
        {
            if (!ModelState.IsValid)
                return ValidationError<QuestionToReturnViewModel>();

            var dto = _mapper.Map<CreateQuestionDto>(model);
            dto.InstructorId = _currentUser.UserId;

            var result = await _questionServices.CreateAsync(dto);
            return ToResponse<QuestionToReturnDto, QuestionToReturnViewModel>(result, "Question created successfully");
         }

        /// <summary>
        /// Updates an existing question
        /// </summary>
        [HttpPut("{questionId:guid}")]
        [ProducesResponseType(typeof(ResponseViewModel<QuestionToReturnViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel<QuestionToReturnViewModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseViewModel<QuestionToReturnViewModel>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseViewModel<QuestionToReturnViewModel>>> Update(
            Guid questionId,
            [FromBody] UpdateQuestionViewModel model)
        {
            if (!ModelState.IsValid)
                return ValidationError<QuestionToReturnViewModel>();

            if (questionId != model.Id || questionId == Guid.Empty)
                return BadRequest(ResponseViewModel<QuestionToReturnViewModel>.Failure(
                    ErrorCode.BadRequest,
                    "Question ID in URL does not match ID in body"));

            var dto = _mapper.Map<UpdateQuestionDto>(model);
            dto.InstructorId = _currentUser.UserId;

            var result = await _questionServices.UpdateAsync(dto);
            return ToResponse<QuestionToReturnDto, QuestionToReturnViewModel>(result, "Question updated successfully");
        }

        /// <summary>
        /// Deletes a question
        /// </summary>
        [HttpDelete("{questionId:guid}")]

        public async Task<ActionResult<ResponseViewModel<Result>>> Delete(Guid questionId)
        {
            if (CheckId<Result>(questionId) is { } badResult) return badResult;

            var result = await _questionServices.DeleteAsync(questionId, _currentUser.UserId);
            return ToResponse(result, "Question deleted successfully", null);
         
        }

    }
}
