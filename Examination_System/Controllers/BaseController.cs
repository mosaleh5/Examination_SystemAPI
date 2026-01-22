using Examination_System.Common;
using Examination_System.ViewModels;
using Examination_System.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;


namespace Examination_System.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class BaseController : ControllerBase
    {
        protected readonly IMapper _mapper;

        protected BaseController(IMapper mapper)
        {
            _mapper = mapper;
        }

        protected string GetValidationErrors()
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(msg => !string.IsNullOrEmpty(msg));

            return string.Join("\n ", errors);
        }

        #region Private Helper Methods
        protected ActionResult<ResponseViewModel<Result>> ToResponse(Result result, string? successMessage = null, string? errorMessage = null)
        {
            if (!result.IsSuccess)
            {
                return ToErrorResponse<Result>(result, errorMessage);
            }
            return Ok(ResponseViewModel<Result>.Success(result, successMessage));
        }
        /// <summary>
        /// Validates a Guid identifier. Returns a BadRequest if the id is null or empty; otherwise, returns null.
        /// </summary>
        protected ActionResult<ResponseViewModel<TViewModel>>? CheckId<TViewModel>(
            Guid? id,
            string? errorMessage = "Id Can not be null or Empty")
        {

            if (id == null || id == Guid.Empty)
                return BadRequest(ResponseViewModel<TViewModel>.Failure(ErrorCode.BadRequest, errorMessage));
            return null;
        }
        protected ActionResult<ResponseViewModel<T>>? CheckIds<T>(params Guid[] ids)
        {
            foreach (var id in ids)
            {
                if (CheckId<T>(id) is { } badResult)
                    return badResult;
            }
            return null;
        }
        protected ActionResult<ResponseViewModel<TViewModel>> ToResponse<TDto, TViewModel>(
            Result<TDto> result,
            string? successMessage = null
            , string? errorMessage = null)
            where TDto : class
        {
            if (!result.IsSuccess)
                return ToErrorResponse<TViewModel>(result, errorMessage);

            var viewModel = _mapper.Map<TViewModel>(result.Data);
            return Ok(ResponseViewModel<TViewModel>.Success(viewModel, successMessage));
        }

        protected ActionResult<ResponseViewModel<IEnumerable<TViewModel>>> ToResponse<TDto, TViewModel>(
            Result<IEnumerable<TDto>> result)
            where TDto : class
        {
            if (!result.IsSuccess)
                return BadRequest(ResponseViewModel<IEnumerable<TViewModel>>.Failure(result.Error, result.ErrorMessage));

            var viewModels = _mapper.Map<IEnumerable<TViewModel>>(result.Data);
            return Ok(ResponseViewModel<IEnumerable<TViewModel>>.Success(viewModels));
        }

        protected ActionResult<ResponseViewModel<T>> ToErrorResponse<T>(Result result, string? errormessage)
        {
            var messages = new[] { errormessage, result.ErrorMessage }
                         .Where(m => !string.IsNullOrWhiteSpace(m));

            string finalMessage = string.Join("\n", messages);

            return BadRequest(ResponseViewModel<T>.Failure(result.Error, finalMessage));
        }
        //=> BadRequest(ResponseViewModel<T>.Failure(result.Error, errormessage + "\n" +result.ErrorMessage));

        protected ActionResult<ResponseViewModel<T>> ValidationError<T>()
            => BadRequest(ResponseViewModel<T>.Failure(ErrorCode.ValidationError, GetValidationErrors()));

        #endregion

    }
}
