namespace Examination_System.Models.Enums
{
    public enum ErrorCode
    {
        NoError = 0,

        None = 0,
        NotFound = 404,
        Unauthorized = 401,
        Forbidden = 403,
        BadRequest = 400,
        Conflict = 409,
        ValidationError = 422,
        InternalServerError = 500,
        FailedToAddQuestionToExam = 501,
        DatabaseError = 502,
        CourseIsNotFound = 503,
        updateQuestion = 504
    }
}
