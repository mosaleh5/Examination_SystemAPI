namespace Examination_System.Services.StudentService
{
    public interface IStudentServices //:IGenericServices<Models.Student, string>
    {
        Task<bool> IsStudentEnrolledInCourseAsync(string studentId, int courseId);
        Task<bool> IsExistsAsync(string Id);
    }
}
