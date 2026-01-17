using AutoMapper;
using Examination_System.DTOs.Course;
using Examination_System.DTOs.Student;
using Examination_System.Models;
using Examination_System.ViewModels.Course;

namespace Examination_System.MappingProfiles
{
    /// <summary>
    /// Mapping profile for Course-related entities
    /// </summary>
    public class CourseMappingProfile : Profile
    {
        public CourseMappingProfile()
        {
            // ViewModel to DTO mappings
            CreateMap<CreateCourseViewModel, CreateCourseDto>();
            CreateMap<UpdateCourseViewModel, UpdateCourseDto>();

            // DTO to Entity mappings
            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.Exams, opt => opt.Ignore());

            // Entity to DTO mappings
            CreateMap<Course, CourseDtoToReturn>();
            CreateMap<CourseEnrollment, CourseEnrollmentDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
                .ForMember(dest => dest.CourseDescription, opt => opt.MapFrom(src => src.Course.Description))
                .ForMember(dest => dest.InstructorId, opt => opt.MapFrom(src => src.Course.InstructorId))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src =>
                    src.Course.Instructor != null
                        ? src.Course.Instructor.User.FirstName + " " + src.Course.Instructor.User.LastName
                        : null));

            // DTO to ViewModel mappings
            CreateMap<CourseDtoToReturn, CourseResponseViewModel>();
        }
    }
}