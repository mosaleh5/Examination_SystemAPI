using Examination_System.Models.Enums;
using Examination_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Examination_System.Filters
{
    public class ValidateGuidFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value is Guid guidValue)
                {
                    if (guidValue == Guid.Empty)
                    {
                        var response = ResponseViewModel<object>.Failure(
                            ErrorCode.BadRequest,
                            $"{argument.Key} cannot be empty."
                        );
                        context.Result = new BadRequestObjectResult(response);
                        return;
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
