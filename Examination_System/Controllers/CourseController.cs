using Examination_System.DTOs.Course;
using Examination_System.Models;
using Examination_System.Services;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{

    public class CourseController : BaseController
    {
        private readonly CourseServices _courseServices;

        public CourseController(CourseServices courseServices)
        {
            _courseServices = courseServices;
        }

        /// <summary>
        /// Get all courses
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CourseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll()
        {
            var courses = await _courseServices.GetAllAsync();
            return Ok(courses);
        }

        /// <summary>
        /// Get course by ID with full details
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CourseDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseDetailsDto>> GetById(int id)
        {
            var course = await _courseServices.GetByIdAsync(id);
            
            if (course == null)
                return NotFound(new { message = $"Course with ID {id} not found" });

            return Ok(course);
        }

        /// <summary>
        /// Get courses by instructor ID
        /// </summary>
        [HttpGet("instructor/{instructorId}")]
        [ProducesResponseType(typeof(IEnumerable<CourseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetByInstructor(int instructorId)
        {
            var courses = await _courseServices.GetCoursesByInstructorAsync(instructorId);
            return Ok(courses);
        }

        /// <summary>
        /// Create a new course
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var course = await _courseServices.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = course.ID }, course);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while creating the course", details = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing course
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseDto>> Update(int id, [FromBody] UpdateCourseDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != updateDto.ID)
                return BadRequest(new { message = "ID in URL does not match ID in body" });

            try
            {
                var updatedCourse = await _courseServices.UpdateAsync(id, updateDto);
                return Ok(updatedCourse);
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
                    new { message = "An error occurred while updating the course", details = ex.Message });
            }
        }

        /// <summary>
        /// Delete a course (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _courseServices.DeleteAsync(id);
            
            if (!result)
                return NotFound(new { message = $"Course with ID {id} not found" });

            return NoContent();
        }

        /// <summary>
        /// Check if course exists
        /// </summary>
        [HttpHead("{id}")]
        [HttpGet("{id}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Exists(int id)
        {
            var exists = await _courseServices.ExistsAsync(id);
            
            if (!exists)
                return NotFound();

            return Ok(new { exists = true });
        }

        /// <summary>
        /// Get enrolled students count for a course
        /// </summary>
        [HttpGet("{id}/students/count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetEnrolledStudentsCount(int id)
        {
            var count = await _courseServices.GetEnrolledStudentsCountAsync(id);
            return Ok(new { courseId = id, enrolledStudentsCount = count });
        }
    }
}
