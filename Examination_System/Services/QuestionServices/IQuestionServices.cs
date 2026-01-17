using Examination_System.Common;
using Examination_System.DTOs.Question;

namespace Examination_System.Services.QuestionServices
{
    /// <summary>
    /// Service for managing questions and their choices
    /// </summary>
    public interface IQuestionServices
    {
        /// <summary>
        /// Creates a new question with choices
        /// </summary>
        Task<Result<QuestionToReturnDto>> CreateAsync(CreateQuestionDto dto);

        /// <summary>
        /// Gets a question by ID for a specific instructor
        /// </summary>
        Task<Result<QuestionToReturnDto>> GetByIdAsync(Guid questionId, Guid instructorId);

        /// <summary>
        /// Gets all questions for an instructor
        /// </summary>
        Task<Result<IEnumerable<QuestionToReturnDto>>> GetByInstructorAsync(Guid instructorId);

        /// <summary>
        /// Gets all questions for an instructor in a specific course
        /// </summary>
        Task<Result<IEnumerable<QuestionToReturnDto>>> GetByInstructorAndCourseAsync(Guid instructorId, Guid courseId);

        /// <summary>
        /// Updates an existing question and its choices
        /// </summary>
        Task<Result<QuestionToReturnDto>> UpdateAsync(UpdateQuestionDto dto);

        /// <summary>
        /// Deletes a question if not used in any exams
        /// </summary>
        Task<Result> DeleteAsync(Guid questionId, Guid instructorId);
    }
}
