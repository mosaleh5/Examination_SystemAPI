using AutoMapper;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.DTOs.Question;
using Examination_System.Models;
using Examination_System.ViewModels.AttemptExam;
using Examination_System.ViewModels.Question;

namespace Examination_System.MappingProfiles
{
    /// <summary>
    /// Mapping profile for Question-related entities
    /// </summary>
    public class QuestionMappingProfile : Profile
    {
        public QuestionMappingProfile()
        {
            // ViewModel to DTO mappings
            CreateMap<CreateQuestionViewModel, CreateQuestionDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));
            CreateMap<UpdateQuestionViewModel, UpdateQuestionDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));

            // DTO to Entity mappings
            CreateMap<CreateQuestionDto, Question>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices))
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForMember(dest => dest.ExamQuestions, opt => opt.Ignore());
            CreateMap<UpdateQuestionDto, Question>();

            // Entity to DTO mappings
            CreateMap<Question, QuestionToReturnDto>();
            CreateMap<Question, QuestionToReturnForStudentDto>();
            CreateMap<Question, QuestionToAttemptDto>();

            // DTO to ViewModel mappings
            CreateMap<QuestionToReturnDto, QuestionToReturnViewModel>();
            CreateMap<QuestionToReturnForStudentDto, QuestionToReturnViewModel>();
            CreateMap<QuestionToAttemptDto, QuestionForAttemptViewModel>();
        }
    }
}