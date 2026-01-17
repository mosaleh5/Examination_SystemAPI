using AutoMapper;
using Examination_System.DTOs.Exam;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.Models;
using Examination_System.ViewModels.Exam;
using Examination_System.ViewModels.AttemptExam;

namespace Examination_System.MappingProfiles
{
    /// <summary>
    /// Mapping profile for Exam-related entities
    /// </summary>
    public class ExamMappingProfile : Profile
    {
        public ExamMappingProfile()
        {
            // ViewModel to DTO mappings
            CreateMap<CreateExamViewModel, CreateExamDto>();
            CreateMap<CreateAutomaticExamViewModel, CreateAutomaticExamDto>();
            CreateMap<AssignStudentToExamDto, ExamAssignment>();

            // DTO to Entity mappings
            CreateMap<CreateExamDto, Exam>();
            CreateMap<CreateAutomaticExamDto, Exam>();

            // Entity to DTO mappings
            CreateMap<Exam, ExamToReturnDto>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src =>
                    src.ExamQuestions.Select(s => s.Question)));
            CreateMap<Exam, ExamToAttemptDto>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src =>
                    src.ExamQuestions.Select(s => s.Question)));
            CreateMap<ExamQuestion, ExamQuestionToReturnDto>();

            // DTO to ViewModel mappings
            CreateMap<ExamToReturnDto, ExamResponseViewModel>();
            CreateMap<ExamToReturnDto, ExamDetailedResponseViewModel>()
                .IncludeBase<ExamToReturnDto, ExamResponseViewModel>();
            CreateMap<ExamToAttemptDto, ExamToAttemptDetailedResponseForStudentViewModel>();
        }
    }
}