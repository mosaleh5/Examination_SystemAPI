using Examination_System.DTOs.Course;
using Examination_System.DTOs.Exam;
using Examination_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    public class ExamController :BaseController
    {
        public ExamServices _examServices { get; }

        public ExamController(ExamServices ExamServices)
        {
            this._examServices = ExamServices;
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExamDto>), StatusCodes.Status200OK)]
        [ProducesResponseType( StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IEnumerable<ExamDto>>> GetAllAsync() {
           return await _examServices.GetAllExamAsync(); 
        }

    }
}
