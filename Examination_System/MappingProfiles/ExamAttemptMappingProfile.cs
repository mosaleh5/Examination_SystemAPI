using AutoMapper;
using Examination_System.DTOs.Exam;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.DTOs.Question;
using Examination_System.Models;
using Examination_System.ViewModels.AttemptExam;
using Examination_System.ViewModels.Choice;
using Examination_System.ViewModels.Exam;
using Examination_System.ViewModels.Question;

namespace Examination_System.MappingProfiles
{
    /// <summary>
    /// Mapping profile for ExamAttempt and StudentAnswer entities
    /// </summary>
    public class ExamAttemptMappingProfile : Profile
    {
        public ExamAttemptMappingProfile()
        {
            // ViewModel to DTO mappings
            CreateMap<SubmitAnswerForStudentViewModel, SubmitAnswerDto>();

            // Entity to DTO mappings
            CreateMap<ExamAttempt, ExamAttemptDto>()
                .ForMember(dest => dest.StudentAnswers, opt => opt.MapFrom(src => src.StudentAnswers))
                .ForMember(dest => dest.ExamTitle, opt => opt.MapFrom(src => src.Exam.Title))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score ?? 0))
                .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => src.MaxScore ?? 0))
                .ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => src.Percentage ?? 0))
                .ForMember(dest => dest.PassingPercentage, opt => opt.Ignore());

            CreateMap<StudentAnswer, AttemptAnswersDto>();
            CreateMap<StudentAnswer, StudentAnswerToReturnDto>();
            CreateMap<StudentAnswer, StudentAnswerDto>()
                .ForMember(dest => dest.ExamAttemtId, opt => opt.MapFrom(src => src.AttemptId))
                .ForMember(dest => dest.ChoiceAnswer, opt => opt.MapFrom(src =>
                    src.SelectedChoice != null ? src.SelectedChoice.Text : string.Empty));

            // DTO to DTO mappings
            CreateMap<AttemptAnswersDto, StudentAnswerToReturnDto>();

            // DTO to ViewModel mappings
            CreateMap<ExamAttemptDto, ExamAttemptResponseForStudentViewModel>()
                .ForMember(dest => dest.StudentAnswers, opt => opt.MapFrom(src => src.StudentAnswers));
            CreateMap<StudentAnswerDto, AttemptAnswerResponseForStudentViewModel>();
            CreateMap<StudentAnswerToReturnDto, AttemptAnswerResponseForStudentViewModel>();

            CreateMap<Exam, ExamToAttemptDto>()
               .ForMember(dest => dest.Questions,
                   opt => opt.MapFrom(src => src.ExamQuestions.Select(eq => eq.Question)))
               .ForMember(dest => dest.ExamId,
               opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ID, opt => opt.Ignore())
                .ForMember(dest => dest.StartedAt, opt => opt.Ignore());

            CreateMap<Question, QuestionToAttemptDto>()

                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));
            CreateMap<Choice, ChoiceToAttemptDto>();

            CreateMap<UpdateQuestionViewModel, UpdateQuestionDto>();
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
            CreateMap<Exam, ExamToAttemptDto>();

            // ExamToAttempt mappings - Missing mapping that causes the exception
            CreateMap<ExamToAttemptDto, ExamToAttemptDetailedResponseForStudentViewModel>();
            CreateMap<QuestionToAttemptDto, QuestionForAttemptViewModel>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Title));
            CreateMap<ChoiceToAttemptDto, ChoiceForAttemptViewModel>();

        }
    }
}