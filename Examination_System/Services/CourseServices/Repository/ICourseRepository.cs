using Examination_System.Models;

namespace Examination_System.Services.CourseServices.Repository
{
    public interface ICourseRepository
    {
        Task<bool> ExistsAsync(Guid courseId);
        Task<bool> IsInstructorAsync(Guid courseId, Guid instructorId);
        Task<Course?> GetByIdAsync(Guid courseId);
        IQueryable<Course> GetByInstructor(Guid instructorId);
        Task<bool> IsStudentAlreadyEnrolledAsync(Guid courseId, Guid studentId);
        Task<bool> IsInstructorOfCourseAsync(Guid courseId, Guid instructorId);
    }
}
