using Examination_System.Common;
using Examination_System.ViewModels;
using Examination_System.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    [ApiController]
    [Route("api/[controller]/[Action]")]
    public class BaseController : ControllerBase
    {
      
        protected  string GetValidationErrors()
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(msg => !string.IsNullOrEmpty(msg));

            return string.Join("; ", errors);
        }
    }
}
