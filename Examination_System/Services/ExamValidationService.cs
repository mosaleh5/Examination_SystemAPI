using Examination_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Examination_System.Services
{
    /// <summary>
    /// Service for validating exam and grade data
    /// </summary>
    public class ExamValidationService
    {
        /// <summary>
        /// Validates exam properties
        /// </summary>
        public static ValidationResult ValidateExam(Exam exam)
        {
            var errors = new List<string>();

            if (exam.Date <= DateTime.Now)
            {
                errors.Add("Exam date must be in the future.");
            }

            if (exam.DurationInMinutes <= 0)
            {
                errors.Add("Duration must be at least 1 minute.");
            }

            if (exam.Fullmark <= 0)
            {
                errors.Add("Full marks must be greater than 0.");
            }

            if (exam.QuestionsCount <= 0)
            {
                errors.Add("Questions count must be at least 1.");
            }

            if (string.IsNullOrWhiteSpace(exam.Title))
            {
                errors.Add("Exam title is required.");
            }

            return errors.Count > 0 
                ? new ValidationResult(string.Join(" ", errors))
                : ValidationResult.Success;
        }

  
    }
}
