using Examination_System.Common;
using Examination_System.DTOs.Course;
using Examination_System.Models;

namespace Examination_System.Services.CourseServices
{
    public interface ICourseServices : IGenericServices<Course>
    {
        Task<Result<IEnumerable<CourseDtoToReturn>>> GetAllForInstructorAsync(Guid instructorId);
        Task<Result<CourseDtoToReturn>> CreateAsync(CreateCourseDto createDto);
        Task<Result<CourseDtoToReturn>> GetByIdAsync(Guid id, Guid instructorId);
     
        Task<Result> EnrollStudentInCourseAsync(CourseEnrollementDto courseEnrollementDto);
        Task<Result<CourseDtoToReturn>> UpdateAsync(UpdateCourseDto updateCourseDto, Guid userId);
        Task<Result> DeleteAsync(Guid courseId, Guid userId);
    
    }
}
