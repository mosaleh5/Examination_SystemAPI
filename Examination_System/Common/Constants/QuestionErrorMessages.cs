namespace Examination_System.Common.Constants
{
    public static class QuestionErrorMessages
    {
        public const string NotFound = "Question with ID {0} not found";
        public const string NotFoundOrNoPermission = "Question with ID {0} not found or you don't have permission to access it";
        public const string NoQuestionsForInstructor = "No questions found for instructor {0}";
        public const string NoQuestionsForInstructorInCourse = "No questions found for instructor {0} in course {1}";
        public const string CourseNotFound = "Course with ID {0} not found";
        public const string CreateFailed = "Failed to create question. Database error occurred.";
        public const string UpdateFailed = "Failed to update question. Database error occurred.";
        public const string DeleteFailed = "Failed to delete question. Database error occurred.";
        public const string UsedInExams = "Cannot delete question with ID {0} because it is used in {1} exam(s)";
    }
}