using Examination_System.DTOs.Course;

namespace Examination_System.Services.CourseServices
{
    public interface ICourseServices : IGenericServices<Examination_System.Models.Course , int>
    {
        Task<IEnumerable<CourseDtoToReturn>> GetAllForInstructorAsync(string InstructorId);
        Task<CourseDtoToReturn> CreateAsync(CreateCourseDto createDto);
       
        Task<CourseDetailsDto> GetByIdAsync(int id);
        Task<bool> IsInstructorOfCourseAsync(int courseId, string? instructorId);
        Task<bool> EnrollStudentInCourseAsync(int courseId, string studentId);
        Task<CourseDtoToReturn>  UpdateAsync(UpdateCourseDto updateCourseDto, string userId);
        Task DeleteAsync(int courseId);
    }
}
