using Examination_System.Services.CurrentUserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Examination_System.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class ValidateUserAuthFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata
                .OfType<AllowAnonymousAttribute>()
                .Any();

            if (allowAnonymous)
            {
                return;
            }
            var currentUserServices = context.HttpContext.RequestServices
                .GetService<ICurrentUserServices>();

            if (currentUserServices?.UserId == null || !currentUserServices.IsAuthenticated)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "User is not authenticated." });
                return;
            } 

            base.OnActionExecuting(context);
        }
    }
}

