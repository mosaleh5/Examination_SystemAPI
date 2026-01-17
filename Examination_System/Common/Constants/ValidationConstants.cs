namespace Examination_System.Common.Constants
{
    /// <summary>
    /// Validation constants used throughout the application
    /// </summary>
    public static class ValidationConstants
    {
        // Password
        public const int MinPasswordLength = 8;
        public const int MaxPasswordLength = 100;

        // Exam
        public const int MinExamDurationMinutes = 5;
        public const int MaxExamDurationMinutes = 300; // 5 hours
        public const int MinQuestionsCount = 1;
        public const int MaxQuestionsCount = 200;
        public const double MinPassingPercentage = 0;
        public const double MaxPassingPercentage = 100;

        // Question
        public const int MinQuestionTitleLength = 10;
        public const int MaxQuestionTitleLength = 500;
        public const int MinChoicesCount = 2;
        public const int MaxChoicesCount = 10;

        // User
        public const int MinNameLength = 2;
        public const int MaxNameLength = 100;
        public const int MinUsernameLength = 3;
        public const int MaxUsernameLength = 50;

        // General
        public const int MaxPageSize = 100;
        public const int DefaultPageSize = 10;
    }
}