using AutoMapper;
using Examination_System.Common;
using Examination_System.DTOs.Course;
using Examination_System.DTOs.Question;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Services;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Services.QuestionServices;
using Examination_System.Validation;
using Examination_System.ViewModels;
using Examination_System.ViewModels.Choice;
using Examination_System.ViewModels.Question;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Examination_System.Controllers
{
    [Authorize(Roles= "Instructor , Admin")]
    [ValidateUserAuthentication]
    public class QuestionController : BaseController
    {
        private readonly IQuestionServices _questionServices;
        private readonly IMapper _mapper;
        private readonly ICurrentUserServices _currentUser;
        private readonly IValidator<CreateQuestionViewModel> _CreateQuestionValidator;
        private readonly IValidator<UpdateQuestionViewModel> _updateQuestionValidator;


        public QuestionController(
            IQuestionServices questionServices,
            IMapper mapper,
            ICurrentUserServices currentUser,
            IValidator<CreateQuestionViewModel> createQuestionValidator,
            IValidator<UpdateQuestionViewModel> updateQuestionValidator)
        {
            _questionServices = questionServices;
            _mapper = mapper;
            _currentUser = currentUser;
            _CreateQuestionValidator = createQuestionValidator;
            _updateQuestionValidator = updateQuestionValidator;
        }

        [HttpGet("{questionId:guid}")]  // Add :guid constraint
        [ProducesResponseType(typeof(ResponseViewModel<QuestionToReturnViewModel>), 200)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), 404)]
        [ProducesResponseType(typeof(ResponseViewModel<object>), 401)]
        public async Task<ActionResult<ResponseViewModel<QuestionToReturnViewModel>>> GetQuestionById(Guid questionId)
        {
      /*      if (string.IsNullOrEmpty(_currentUser.UserId))
                return Unauthorized(ResponseViewModel<QuestionToReturnDto>.Failure(
                    Models.Enums.ErrorCode.Unauthorized,
                    "User not authenticated"));*/

            var result = await _questionServices.GetQuestionByIdAsync(
                questionId, 
                _currentUser.UserId);
            var Data = _mapper.Map<QuestionToReturnDto, QuestionToReturnViewModel>(result.Data);
            return result.IsSuccess ? Ok(ResponseViewModel<QuestionToReturnViewModel>.Success(Data)):
                BadRequest(ResponseViewModel<QuestionToReturnViewModel>.Failure(result.Error,result.ErrorMessage));
        }

        [HttpGet]
       
        public async Task<ActionResult<ResponseViewModel<IEnumerable<QuestionToReturnViewModel>>>> GetQuestionsByInstructor()
        {
            var result = await _questionServices.GetQuestionsByInstructorAsync(_currentUser.UserId);
            var Data = _mapper.Map<IEnumerable<QuestionToReturnDto>,IEnumerable<QuestionToReturnViewModel>>(result.Data);

            return result.IsSuccess? Ok(ResponseViewModel<IEnumerable<QuestionToReturnViewModel>>.Success(Data)):
                BadRequest(ResponseViewModel<QuestionToReturnViewModel>.Failure(result.Error , result.ErrorMessage));
        }

        [HttpPost]
      
        public async Task<ActionResult<ResponseViewModel<QuestionToReturnViewModel>>> CreateQuestion(
            [FromBody] CreateQuestionViewModel model)
        {
         
            if (!ModelState.IsValid)
                return BadRequest(ResponseViewModel<QuestionToReturnViewModel>.Failure(
                    Models.Enums.ErrorCode.ValidationError,
                    GetValidationErrors()));



            var dto = _mapper.Map<CreateQuestionDto>(model);
            dto.InstructorId = _currentUser.UserId;
            
            var result = await _questionServices.CreateQuestionAsync(dto);
            
            var mappedData = _mapper.Map<QuestionToReturnDto, QuestionToReturnViewModel>(result.Data);
           
            return result.IsSuccess ? CreatedAtAction(
                nameof(GetQuestionById), 
                new { questionId = result.Data.Id}, 
                ResponseViewModel<QuestionToReturnViewModel>.Success(mappedData, "Question created successfully")):
                BadRequest(ResponseViewModel<QuestionToReturnViewModel>.Failure(result.Error ,"An Error Aqure When Create Course \n  " + result.ErrorMessage ));
        }

        [HttpPut("{questionId:guid}")]
        [ProducesResponseType(typeof(ResponseViewModel<QuestionToReturnDto>), 200)]
        [ProducesResponseType(typeof(ResponseViewModel<QuestionToReturnDto>), 422)]
        [ProducesResponseType(typeof(ResponseViewModel<QuestionToReturnDto>), 404)]
        public async Task<ActionResult<ResponseViewModel<QuestionToReturnViewModel>>> UpdateQuestion(
            Guid questionId, 
            [FromBody] UpdateQuestionViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseViewModel<QuestionToReturnViewModel>.Failure(
                    Models.Enums.ErrorCode.ValidationError,
                    GetValidationErrors()));

            if (questionId != model.Id)
                return BadRequest(ResponseViewModel<QuestionToReturnViewModel>.Failure(
                    ErrorCode.BadRequest,
                    "Question ID mismatch"));


            var dto = _mapper.Map<UpdateQuestionViewModel , UpdateQuestionDto>(model);
            dto.InstructorId = _currentUser.UserId;
            
            var result = await _questionServices.UpdateQuestionAsync(dto);
            var Data = _mapper.Map<QuestionToReturnDto, QuestionToReturnViewModel>(result.Data);

            return result.IsSuccess ? Ok(ResponseViewModel<QuestionToReturnViewModel>.Success(Data, "Question updated successfully")):
                BadRequest(ResponseViewModel<QuestionToReturnViewModel>.Failure(result.Error, result.ErrorMessage));
        }

        [HttpDelete("{questionId:guid}")]
        [ProducesResponseType(typeof(ResponseViewModel<Result>), 200)]
        [ProducesResponseType(typeof(ResponseViewModel<Result>), 404)]
        public async Task<ActionResult<ResponseViewModel<Result>>> DeleteQuestion(Guid questionId)
        {


            var result = await _questionServices.DeleteQuestionAsync(
                questionId, 
                _currentUser.UserId);
            
            return result.IsSuccess? Ok(ResponseViewModel<Result>.Success(result, "Question deleted successfully")):
                BadRequest(ResponseViewModel<Result>.Failure(result.Error, " An Error Happen When Delete Question\n" + result.ErrorMessage));
        }
    }
}
