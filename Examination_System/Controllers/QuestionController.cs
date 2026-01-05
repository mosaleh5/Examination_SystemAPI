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

        [HttpGet("question/{questionId}")]
        [ProducesResponseType(typeof(QuestionToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<QuestionToReturnDto>> GetQuestionById(int questionId)
        {
            var instructorId = _currentUser.UserId;
            if (!_currentUser.IsAuthenticated || instructorId.IsNullOrEmpty()) 
                return Unauthorized(new { message = "User is not authenticated." });

            try
            {
                var question = await _questionServices.GetQuestionByIdAsync(questionId, instructorId);
                return Ok(question);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while retrieving the question", details = ex.Message });
            }
        }

        [HttpPut("{questionId}")]
        [ProducesResponseType(typeof(QuestionToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<QuestionToReturnDto>> UpdateQuestion(int questionId, [FromBody] UpdateQuestionViewModel updateQuestionViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var instructorId = _currentUser.UserId;
            if (!_currentUser.IsAuthenticated || instructorId.IsNullOrEmpty()) 
                return Unauthorized(new { message = "User is not authenticated." });

            if (questionId != updateQuestionViewModel.Id)
            {
                return BadRequest(new { message = "Question ID mismatch." });
            }

            try
            {
                var updateQuestionDto = _mapper.Map<UpdateQuestionViewModel, UpdateQuestionDto>(updateQuestionViewModel);
                updateQuestionDto.InstructorId = instructorId;

                var updatedQuestion = await _questionServices.UpdateQuestionAsync(updateQuestionDto);
                return Ok(updatedQuestion);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while updating the question", details = ex.Message });
            }
        }

        [HttpDelete("{questionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult> DeleteQuestion(int questionId)
        {
            var instructorId = _currentUser.UserId;
            if (!_currentUser.IsAuthenticated || instructorId.IsNullOrEmpty()) 
                return Unauthorized(new { message = "User is not authenticated." });

            if (questionId <= 0)
            {
                return BadRequest(new { message = "Invalid question ID." });
            }

            try
            {
                var result = await _questionServices.DeleteQuestionAsync(questionId, instructorId);
                return Ok(new { message = "Question deleted successfully.", questionId = questionId });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while deleting the question", details = ex.Message });
            }
        }
    }
}
