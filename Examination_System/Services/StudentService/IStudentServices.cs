using Examination_System.Common;
using Examination_System.DTOs.Student;

namespace Examination_System.Services.StudentService
{
    public interface IStudentServices
    {
        Task<Result> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId);
        Task<Result<IEnumerable<CourseEnrollmentDto>>> GetEnrolledCoursesAsync(Guid studentId);
        Task<Result<IEnumerable<ExamAssignmentDto>>> GetAssignedExamsAsync(Guid studentId);
        Task<Result<StudentDetailsDto>> GetStudentDetailsAsync(Guid studentId);
    }
}