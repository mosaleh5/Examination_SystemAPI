using Examination_System.Models.Enums;
using Examination_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Examination_System.Filters.ValidationFilters
{
    public class ValidateModelStateFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
                
                var response = ResponseViewModel<object>.Failure(
                    ErrorCode.ValidationError,
                    string.Join("\n", errors)
                );

                context.Result = new BadRequestObjectResult(response);
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {   
        }
    }
}
