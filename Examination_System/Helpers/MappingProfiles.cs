using AutoMapper;
using Examination_System.DTOs.Course;
using Examination_System.DTOs.Exam;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.DTOs.Question;
using Examination_System.Models;
using Examination_System.ViewModels.AttemptExam;
using Examination_System.ViewModels.Choice;
using Examination_System.ViewModels.Course;
using Examination_System.ViewModels.Exam;
using Examination_System.ViewModels.Question;

namespace Examination_System.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Course mappings
            CreateMap<CreateCourseViewModel, CreateCourseDto>();
            CreateMap<Course, CourseDtoToReturn>();
            CreateMap<CreateCourseDto, Course>();
            CreateMap<CourseDtoToReturn, CourseResponseViewModel>();
            CreateMap<UpdateCourseViewModel, UpdateCourseDto>();
            CreateMap<UpdateCourseDto, Course>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.Exams, opt => opt.Ignore());

            // Choice mappings - CRITICAL ORDER!
            CreateMap<ChoiceViewModel, ChoiceDto>(); // ViewModel → DTO
            CreateMap<ChoiceDto, Choice>()           // DTO → Entity
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore()) // Set manually
                .ForMember(dest => dest.Question, opt => opt.Ignore())
                .ForMember(dest => dest.StudentAnswers, opt => opt.Ignore());

            // Question mappings
            CreateMap<CreateQuestionViewModel, CreateQuestionDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices)); // Map nested collection

            CreateMap<CreateQuestionDto, Question>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices)) // Map nested collection
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForMember(dest => dest.ExamQuestions, opt => opt.Ignore());

            CreateMap<UpdateQuestionViewModel, UpdateQuestionDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));

            CreateMap<UpdateQuestionDto, Question>()
                .ForMember(dest => dest.Choices, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForMember(dest => dest.ExamQuestions, opt => opt.Ignore());

            CreateMap<Choice, ChoiceToReturnForInstructorDto>();
            CreateMap<Question, QuestionToReturnDto>();
            CreateMap<Choice, ChoiceToReturnForStudentDto>();
            CreateMap<Question, QuestionToReturnForStudentDto>();
            CreateMap<ChoiceToReturnForInstructorDto , ChoiceToReturnViewModel>();
            CreateMap< QuestionToReturnDto , QuestionToReturnViewModel>();

            CreateMap<ChoiceToReturnForStudentDto, ChoiceToReturnViewModel>();
            CreateMap<QuestionToReturnForStudentDto, QuestionToReturnViewModel>();

            //Exam mappings
            CreateMap<CreateExamViewModel, CreateExamDto>();
            CreateMap<CreateExamDto, Exam>();
            CreateMap<Exam, ExamToReturnDto>()
                .ForMember(dest => dest.ExamQuestions, 
                    opt => opt.MapFrom(src => src.ExamQuestions.Select(eq => eq.Question)))
                .ForMember(dest => dest.CourseName, 
                    opt => opt.MapFrom(src => src.Course.Name))
                .ForMember(dest => dest.InstructorName,
                    opt => opt.MapFrom(src => (src.Instructor.User.FirstName + " " + src.Instructor.User.LastName)));

            CreateMap<ExamQuestion, ExamQuestionToReturnDto>();

            CreateMap<ExamToReturnDto, ExamResponseViewModel>();
            CreateMap<Exam , ExamToAttemptDto>()
                .ForMember(dest => dest.ExamQuestions,
                    opt => opt.MapFrom(src => src.ExamQuestions.Select(eq => eq.Question)))
                .ForMember(dest => dest.InstructorName , 
                    opt=>opt.MapFrom(src=>src.Instructor.User.FirstName +" " + src.Instructor.User.LastName));
            //ExamAttempt mappings
            CreateMap<ExamToAttemptDto, ExamToAttemptDetailedResponseForStudentViewModel>();

            CreateMap<ExamAttempt, ExamAttemptDto>();

            CreateMap<StudentAnswer, AttemptAnswersDto>()
                .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question))
                .ForMember(dest => dest.SelectedChoiceText, opt => opt.MapFrom(src => src.SelectedChoice != null ? src.SelectedChoice.Text : null));
            CreateMap<ExamAttemptDto, ExamAttemptResponseForStudentViewModel>()
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.StudentAnswers));
            CreateMap<AttemptAnswersDto, AttemptAnswerResponseForStudentViewModel>()
                .ForMember(dest => dest.ChoiceAnswer, opt => opt.MapFrom(src => src.SelectedChoiceText));
            CreateMap<SubmitAnswerForStudentViewModel, SubmitAnswerDto>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.choiceId, opt => opt.MapFrom(src => src.AnswerId));
          

        }
    }
}