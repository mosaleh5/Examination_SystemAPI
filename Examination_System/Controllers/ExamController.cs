using AutoMapper;
using Examination_System.DTOs.Exam;
using Examination_System.Services.CourseServices;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Services.ExamServices;
using Examination_System.ViewModels.Exam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Examination_System.Controllers
{
    public class ExamController : BaseController
    {
        public readonly Services.ExamServices.IExamServices _examServices ;
        private readonly ICourseServices _courseServices;
        private readonly ICurrentUserServices _currentUserServices;
        private readonly IMapper _mapper;

        public ExamController(Services.ExamServices.IExamServices ExamServices,
            ICourseServices courseServices,
            ICurrentUserServices currentUser,
            IMapper mapper)
        {
            _examServices = ExamServices;
            this._courseServices = courseServices;
            this._currentUserServices = currentUser;
            this._mapper = mapper;
        }

        [HttpPost("manual")]
        [ProducesResponseType(typeof(ExamResponseViewModel), 200)]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<ExamResponseViewModel>> CreateExam([FromBody] CreateExamViewModel createExamViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
         
            var instructorId = _currentUserServices.UserId;
            if (_currentUserServices.UserId.IsNullOrEmpty() || !_currentUserServices.IsAuthenticated)
            { 
                return Unauthorized("User is not authenticated");
            }
          
            var createExamDto = _mapper.Map<CreateExamDto>(createExamViewModel);
            createExamDto.InstructorId = instructorId;
            createExamDto.CreatedAt = DateTime.UtcNow;
            var createdExam = await _examServices.CreateExam(createExamDto);
            var examViewModel = _mapper.Map<ExamResponseViewModel>(createdExam);
            return Ok(examViewModel);
           
        }

        [HttpPost("automatic")]
        [ProducesResponseType(typeof(ExamDetailedResponseViewModel), 201)]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<ExamDetailedResponseViewModel>> CreateAutomaticExam([FromBody] CreateAutomaticExamViewModel createAutomaticExamViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var courseExists = await _courseServices.IsExistsAsync(createAutomaticExamViewModel.CourseId);
            if (!courseExists)
            {
                return NotFound($"Course with Id {createAutomaticExamViewModel.CourseId} does not exist");
            }
            var instructorId = _currentUserServices.UserId;
            if (_currentUserServices.UserId.IsNullOrEmpty() || !_currentUserServices.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated");
            }
            var isInstructorOfCourse = await _courseServices.IsInstructorOfCourseAsync(createAutomaticExamViewModel.CourseId, instructorId);
            if (!isInstructorOfCourse)
            {
                return BadRequest("You are not allowed to sign exam to this course");
            }
            var createAutomaticExamDto = _mapper.Map<CreateAutomaticExamDto>(createAutomaticExamViewModel);
            createAutomaticExamDto.InstructorId = instructorId;
            createAutomaticExamDto.CreatedAt = DateTime.UtcNow;
            var createdExam = await _examServices.CreateAutomaticExam(createAutomaticExamDto);
            var createdExamViewModel = _mapper.Map<ExamDetailedResponseViewModel>(createdExam);
            return Ok(createdExamViewModel);

        }

        [HttpGet]
        [Authorize(Roles = "Instructor")]
        [ProducesResponseType(typeof(IEnumerable<ExamResponseViewModel>), 200)]
        public async Task<ActionResult<IEnumerable<ExamResponseViewModel>>> GetAllExamsForInstructor()
        {
            var instructorId = _currentUserServices.UserId;
            if (_currentUserServices.UserId.IsNullOrEmpty() || !_currentUserServices.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated");
            }
        
            var exams = await _examServices.GetAllExamsForInstructor(instructorId);
            var examViewModels = _mapper.Map<IEnumerable<ExamResponseViewModel>>(exams);
            return Ok(examViewModels);

        }

        [HttpPost("manual/{examId}/questions")]
        [Authorize(Roles = "Instructor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddQuestionsToExam(int examId, [FromBody] List<int> questionIds)
        {
            var userId = _currentUserServices.UserId;
            if (userId == null || !_currentUserServices.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            if (examId <= 0)
            {
                return BadRequest(new { message = "Invalid exam Id." });
            }

            if (questionIds == null || questionIds.Count == 0)
            {
                return BadRequest(new { message = "Question IDs cannot be null or empty." });
            }

            try
            {
                if (!await _examServices.IsInstructorOfExamAsync(examId, userId))
                {
                    return Unauthorized(new { message = "You are not allowed to add questions to this exam." });
                }

                await _examServices.AddQuestionsToExamAsync(examId, questionIds);
                return Ok(new { message = $"{questionIds.Count} question(s) added to Exam {examId} successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while adding questions to the exam", details = ex.Message });
            }
        }

  
        [HttpPut("manual/{examId}/questions")]
        [Authorize(Roles = "Instructor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReplaceExamQuestions(int examId, [FromBody] List<int> questionIds)
        {
            var userId = _currentUserServices.UserId;
            if (userId == null || !_currentUserServices.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            if (examId <= 0)
            {
                return BadRequest(new { message = "Invalid exam Id." });
            }

            if (questionIds == null || questionIds.Count == 0)
            {
                return BadRequest(new { message = "Question IDs cannot be null or empty." });
            }

            try
            {
                if (!await _examServices.IsInstructorOfExamAsync(examId, userId))
                {
                    return Unauthorized(new { message = "You are not allowed to modify questions in this exam." });
                }

                await _examServices.ReplaceExamQuestionsAsync(examId, questionIds);
                return Ok(new { message = $"Exam {examId} questions replaced successfully with {questionIds.Count} question(s)." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while replacing questions in the exam", details = ex.Message });
            }
        }

    
        [HttpDelete("manual/{examId}/questions/{questionId}")]
        [Authorize(Roles = "Instructor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveQuestionFromExam(int examId, int questionId)
        {
            var userId = _currentUserServices.UserId;
            if (userId == null || !_currentUserServices.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            if (examId <= 0)
            {
                return BadRequest(new { message = "Invalid exam Id." });
            }

            if (questionId <= 0)
            {
                return BadRequest(new { message = "Invalid question Id." });
            }

            try
            {
                if (!await _examServices.IsInstructorOfExamAsync(examId, userId))
                {
                    return Unauthorized(new { message = "You are not allowed to remove questions from this exam." });
                }

                await _examServices.RemoveQuestionFromExamAsync(examId, questionId);
                return Ok(new { message = $"Question {questionId} removed from Exam {examId} successfully." });
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
                    new { message = "An error occurred while removing the question from the exam", details = ex.Message });
            }
        }



        [HttpPut("Activate/{examId}")]
        [Authorize(Roles = "Instructor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ActivateExam(int examId)
        {
            var userId = _currentUserServices.UserId;
            if (userId == null || !_currentUserServices.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }
            
            if (examId <= 0)
            {
                return BadRequest(new { message = "Invalid exam Id." });
            }
            
            try
            {
                await _examServices.ActivateExamAsync(examId, userId);
                return Ok(new { message = $"Exam {examId} activated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while activating the exam", details = ex.Message });
            }
        }




        [HttpPost("{examId}/students")]
        [Authorize(Roles = "Instructor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AssignStudentToExam(int examId, string studentId)
        {
            var userId = _currentUserServices.UserId;
            if (userId == null || !_currentUserServices.IsAuthenticated)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            if (string.IsNullOrEmpty(studentId))
            {
                return BadRequest(new { message = "StudentId cannot be null or empty." });
            }
            
            if (examId <= 0)
            {
                return BadRequest(new { message = "Invalid exam Id." });
            }

            try
            {
                if (!await _examServices.IsInstructorOfExamAsync(examId, userId))
                {
                    return Unauthorized(new { message = "You are not allowed to assign students to this exam." });
                }

                await _examServices.EnrollStudentToExamAsync(examId, studentId);
                return Ok(new { message = $"Student with ID {studentId} enrolled in Exam {examId} successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while enrolling the student in the Exam", details = ex.Message });
            }
        }

       


    }
}
