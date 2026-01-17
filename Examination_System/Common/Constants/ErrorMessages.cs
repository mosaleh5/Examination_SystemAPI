namespace Examination_System.Common.Constants
{
    /// <summary>
    /// Centralized error messages used throughout the application
    /// </summary>
    public static class ErrorMessages
    {
        // Authentication & Authorization
        public const string UserNotAuthenticated = "User not authenticated";
        public const string UnauthorizedAccess = "You do not have permission to access this resource";
        public const string InvalidCredentials = "Invalid username or password";

        // Validation
        public const string ValidationError = "One or more validation errors occurred";
        public const string InvalidInput = "Invalid input provided";
        public const string RequiredField = "{0} is required";
        public const string InvalidFormat = "{0} has invalid format";
        public const string InvalidRange = "{0} must be between {1} and {2}";

        // Exam
        public const string ExamNotFound = "Exam with ID {0} not found";
        public const string ExamNotActive = "Exam {0} is not currently active";
        public const string ExamAlreadyCompleted = "You have already completed this final exam {0}";
        public const string ExamStartFailed = "Failed to start exam. Database error occurred.";
        public const string ExamSubmitFailed = "Failed to submit exam. Database error occurred.";
        public const string InvalidExamDuration = "Exam duration must be positive";
        public const string InvalidPassingPercentage = "Passing percentage must be between 0 and 100";

        // Exam Attempt
        public const string ExamAttemptNotFound = "Exam attempt with ID {0} not found";
        public const string ExamAttemptAlreadySubmitted = "Exam attempt {0} has already been submitted";
        public const string ExamTimeExceeded = "Submission time exceeded. Exam duration: {0} minutes, Actual time: {1:F2} minutes";
        public const string InvalidAnswerCount = "Invalid number of answers. Expected: {0}, Received: {1}";
        public const string AnswersRequired = "Answers cannot be null or empty";
        public const string NoAttemptsFound = "No attempts found for {0}";

        // Student
        public const string StudentNotEnrolled = "Student {0} is not assigned to exam {1}";
        public const string StudentNotFound = "Student with ID {0} not found";

        // Instructor
        public const string InstructorNotFound = "Instructor with ID {0} not found";
        public const string NoStudentAttemptsFound = "No student attempts found for instructor {0}";

        // Question
        public const string QuestionNotFound = "Question with ID {0} not found";
        public const string InvalidQuestionCount = "Invalid question count";

        // Database
        public const string DatabaseError = "A database error occurred";
        public const string SaveFailed = "Failed to save changes";
        public const string UpdateFailed = "Failed to update {0}";
        public const string DeleteFailed = "Failed to delete {0}";

        // General
        public const string InternalServerError = "An unexpected error occurred";
        public const string ResourceNotFound = "{0} not found";
        public const string OperationFailed = "{0} operation failed";
    }
}