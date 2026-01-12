using AutoMapper;
using Examination_System.DTOs.Question;
using Examination_System.DTOs.Exam;
using Examination_System.ViewModels.Question;
using Examination_System.ViewModels.Choice;
using Examination_System.ViewModels.Exam;
using Examination_System.ViewModels.AttemptExam;
using Examination_System.DTOs.Exam;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.Models;

namespace Examination_System.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Question mappings
            CreateMap<QuestionToReturnDto, QuestionToReturnViewModel>();
            CreateMap<CreateQuestionViewModel, CreateQuestionDto>();
            CreateMap<UpdateQuestionViewModel, UpdateQuestionDto>();

            // Choice mappings - QuestionId doesn't exist in ChoiceDto, so ignore it
            CreateMap<ChoiceDto, ChoiceToReturnViewModel>()
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore());
            
            // Exam mappings
            CreateMap<CreateExamViewModel, CreateExamDto>();
            CreateMap<CreateAutomaticExamViewModel, CreateAutomaticExamDto>();
            CreateMap<ExamToReturnDto, ExamResponseViewModel>();
            CreateMap<ExamToReturnDto, ExamDetailedResponseViewModel>()
                .IncludeBase<ExamToReturnDto, ExamResponseViewModel>();
            CreateMap<QuestionToReturnDto, QuestionToReturnViewModel>();

            // ExamAttempt mappings - Entity to DTO
            CreateMap<ExamAttempt, ExamAttemptDto>();
            CreateMap<StudentAnswer, StudentAnswerDto>();
            
            // ExamAttempt mappings - DTO to ViewModel
            CreateMap<ExamAttemptDto, ExamAttemptResponseForStudentViewModel>();
            CreateMap<StudentAnswerDto, AttemptAnswerResponseForStudentViewModel>();
            
            // SubmitAnswer mappings
            CreateMap<SubmitAnswerForStudentViewModel, SubmitAnswerDto>();
        }
    }
}