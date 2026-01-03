using AutoMapper;
using Examination_System.DTOs.Course;
using Examination_System.DTOs.Question;
using Examination_System.Models;
using Examination_System.Services;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Services.QuestionServices;
using Examination_System.ViewModels.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Examination_System.Controllers
{

    public class QuestionController : BaseController
    {
        private readonly IQuestionServices _questionServices;
        private readonly IMapper _mapper;
        private readonly ICurrentUserServices _currentUser;

        public QuestionController(IQuestionServices questionServices,
            IMapper mapper,
            ICurrentUserServices currentUser)
        {
            _questionServices = questionServices;
            _mapper = mapper;
            this._currentUser = currentUser;
        }


      

        [HttpPost]
        [ProducesResponseType(typeof(QuestionToReturnDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<QuestionToReturnDto>> CreateQuestion([FromBody] CreateQuestionViewModel createQuestionViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var instructorId = _currentUser.UserId;
            if (!_currentUser.IsAuthenticated || instructorId.IsNullOrEmpty()) return Unauthorized();
            var createQuestionDto = _mapper.Map<CreateQuestionViewModel, CreateQuestionDto>(createQuestionViewModel);

            createQuestionDto.InstructorId = instructorId;

            var createdQuestion = await _questionServices.CreateQuestionAsync(createQuestionDto);

            //   return CreatedAtAction(nameof(CreateQuestion), new { id = createdQuestion.Id }, createdQuestion);

            return Ok(createdQuestion);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<QuestionToReturnDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<IEnumerable<QuestionToReturnDto>>> GetQuestionsByInstructor()
        {
            var instructorId = _currentUser.UserId;
            if (!_currentUser.IsAuthenticated || instructorId.IsNullOrEmpty()) return Unauthorized();
            var questions = await _questionServices.GetQuestionsByInstructorAsync(instructorId);
            if (questions == null || !questions.Any())
            {
                return NotFound("No questions found for the instructor.");
            }
            return Ok(questions);
        }


        [HttpGet("{CourseId}")]
        [ProducesResponseType(typeof(IEnumerable<QuestionToReturnDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<IEnumerable<QuestionToReturnDto>>> GetQuestionsByInstructorForCourse(int CourseId)
        {
            var instructorId = _currentUser.UserId;
            if (!_currentUser.IsAuthenticated || instructorId.IsNullOrEmpty()) return Unauthorized();
            var questions = await _questionServices.GetQuestionsByInstructorAndCourseAsync(instructorId, CourseId);
            if (questions == null || !questions.Any())
            {
                return NotFound("No questions found for the instructor.");
            }
            return Ok(questions);
        }

    }
}
