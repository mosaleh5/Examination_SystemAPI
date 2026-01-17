using AutoMapper;
using Examination_System.DTOs.Question;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.Models;
using Examination_System.ViewModels.Choice;
using Examination_System.ViewModels.AttemptExam;
using Examination_System.ViewModels.Question;

namespace Examination_System.MappingProfiles
{
    /// <summary>
    /// Mapping profile for Choice-related entities
    /// </summary>
    public class ChoiceMappingProfile : Profile
    {
        public ChoiceMappingProfile()
        {
            // ViewModel to DTO mappings
            CreateMap<CreateChoiceViewModel, ChoiceDto>();
            CreateMap<ChoiceViewModel, ChoiceDto>();
            CreateMap<UpdateChoiceViewModel, UpdateChoiceDto>();

            // DTO to Entity mappings
            CreateMap<ChoiceDto, Choice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
                .ForMember(dest => dest.Question, opt => opt.Ignore())
                .ForMember(dest => dest.StudentAnswers, opt => opt.Ignore());
            CreateMap<UpdateChoiceDto, Choice>();

            // Entity to DTO mappings
            CreateMap<Choice, ChoiceDto>();
            CreateMap<Choice, ChoiceToReturnForInstructorDto>();
            CreateMap<Choice, ChoiceToReturnForStudentDto>();
            CreateMap<Choice, ChoiceToAttemptDto>();

            // DTO to ViewModel mappings
            CreateMap<ChoiceDto, ChoiceViewModel>();
            CreateMap<ChoiceToReturnForInstructorDto, ChoiceViewModel>();
            CreateMap<ChoiceToReturnForStudentDto, ChoiceViewModel>();
            CreateMap<ChoiceDto, ChoiceToReturnViewModel>()
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore());
            CreateMap<ChoiceToAttemptDto, ChoiceForAttemptViewModel>();
        }
    }
}