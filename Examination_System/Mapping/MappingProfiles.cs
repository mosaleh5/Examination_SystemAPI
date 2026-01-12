using AutoMapper;
using Examination_System.Common;
using Examination_System.DTOs.Course;
using Examination_System.DTOs.Exam;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.DTOs.Question;
using Examination_System.DTOs.Student;
using Examination_System.Models;
using Examination_System.ViewModels;
using Examination_System.ViewModels.AttemptExam;
using Examination_System.ViewModels.Choice;
using Examination_System.ViewModels.Course;
using Examination_System.ViewModels.Exam;
using Examination_System.ViewModels.Question;
using ChoiceViewModel = Examination_System.ViewModels.Choice.ChoiceViewModel;

namespace Examination_System.Mapping
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
            CreateMap<CourseEnrollment, CourseEnrollmentDto>()
    .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
    .ForMember(dest => dest.CourseDescription, opt => opt.MapFrom(src => src.Course.Description))
    .ForMember(dest => dest.InstructorId, opt => opt.MapFrom(src => src.Course.InstructorId))
    .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Course.Instructor != null ? src.Course.Instructor.User.FirstName + " " + src.Course.Instructor.User.LastName : null));
            // Choice mappings - CRITICAL ORDER!
            CreateMap<CreateChoiceViewModel, ChoiceDto>(); // ADD THIS LINE - Maps CreateChoiceViewModel → ChoiceDto
            CreateMap<ViewModels.Choice.ChoiceViewModel, ChoiceDto>(); // ViewModel → DTO (fully qualified)
            CreateMap<ChoiceDto, Choice>()           // DTO → Entity
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore()) // Set manually
                .ForMember(dest => dest.Question, opt => opt.Ignore())
                .ForMember(dest => dest.StudentAnswers, opt => opt.Ignore());

            // Question mappings
            CreateMap<CreateQuestionViewModel, CreateQuestionDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));

            CreateMap<CreateQuestionDto, Question>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices))
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForMember(dest => dest.ExamQuestions, opt => opt.Ignore());

            CreateMap<UpdateQuestionViewModel, UpdateQuestionDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));
            CreateMap<Question, QuestionToReturnDto>();
            CreateMap<Question, QuestionToReturnForStudentDto>();
            CreateMap<QuestionToReturnForStudentDto, QuestionToReturnViewModel>();
            CreateMap<QuestionToReturnDto, QuestionToReturnViewModel>();
            CreateMap<UpdateQuestionViewModel, UpdateQuestionDto>();
            CreateMap<UpdateChoiceViewModel, UpdateChoiceDto>();

            CreateMap<ChoiceDto, ChoiceViewModel>();

            CreateMap<UpdateQuestionDto, Question>();
            CreateMap<UpdateChoiceDto, Choice>();
            CreateMap<Choice, ChoiceToReturnForInstructorDto>();
            CreateMap<Choice, ChoiceToReturnForStudentDto>();
            CreateMap<ChoiceToReturnForInstructorDto, ChoiceViewModel>();
            CreateMap<Choice, ChoiceDto>();
            CreateMap<ChoiceToReturnForStudentDto, ChoiceViewModel>();

            //Exam mappings
            CreateMap<CreateExamViewModel, CreateExamDto>();
            CreateMap<CreateExamDto, Exam>();
            CreateMap<Exam, ExamToReturnDto>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.ExamQuestions.Select(s => s.Question)));

            CreateMap<ExamQuestion, ExamQuestionToReturnDto>();

            CreateMap<ExamToReturnDto, ExamResponseViewModel>();
            CreateMap<Exam, ExamToAttemptDto>()
                 .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.ExamQuestions.Select(s => s.Question)));
            CreateMap<Question, QuestionToAttemptDto>();
            CreateMap<Choice, ChoiceToAttemptDto>();
            CreateMap<CreateAutomaticExamDto, Exam>();
            //ExamAttempt mappings
            CreateMap<ExamToAttemptDto, ExamToAttemptDetailedResponseForStudentViewModel>();
            CreateMap<QuestionToAttemptDto, QuestionForAttemptViewModel>();
            CreateMap<ChoiceToAttemptDto, ChoiceForAttemptViewModel>();

            CreateMap<ExamAttempt, ExamAttemptDto>()
                .ForMember(dest => dest.StudentAnswers, opt => opt.MapFrom(src => src.StudentAnswers));

            CreateMap<StudentAnswer, AttemptAnswersDto>();
            /* .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId))
             .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question))
             .ForMember(dest => dest.SelectedChoiceId, opt => opt.MapFrom(src => src.SelectedChoiceId))
             .ForMember(dest => dest.SelectedChoiceText, opt => opt.MapFrom(src => src.SelectedChoice != null ? src.SelectedChoice.Text : null))
             .ForMember(dest => dest.IsCorrect, opt => opt.MapFrom(src => src.IsCorrect));*/

            CreateMap<AttemptAnswersDto, StudentAnswerToReturnDto>();
            /*          .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId))
                      .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question))
                      .ForMember(dest => dest.ChoiceAnswer, opt => opt.MapFrom(src => src.SelectedChoiceText))
                      .ForMember(dest => dest.IsCorrect, opt => opt.MapFrom(src => src.IsCorrect));*/

            CreateMap<StudentAnswer, StudentAnswerToReturnDto>();
            /*   .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId))
               .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question))
               .ForMember(dest => dest.ChoiceAnswer, opt => opt.MapFrom(src => src.SelectedChoice != null ? src.SelectedChoice.Text : null))
               .ForMember(dest => dest.IsCorrect, opt => opt.MapFrom(src => src.IsCorrect));*/

            CreateMap<ExamAttemptDto, ExamAttemptResponseForStudentViewModel>()
                .ForMember(dest => dest.StudentAnswers, opt => opt.MapFrom(src => src.StudentAnswers));

            CreateMap<StudentAnswerToReturnDto, AttemptAnswerResponseForStudentViewModel>();
            /* .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId))
             .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question))
             .ForMember(dest => dest.ChoiceAnswer, opt => opt.MapFrom(src => src.ChoiceAnswer))
             .ForMember(dest => dest.IsCorrect, opt => opt.MapFrom(src => src.IsCorrect));*/
            CreateMap<SubmitAnswerForStudentViewModel, SubmitAnswerDto>();
            CreateMap<ExamAttempt, ExamAttemptDto>()
                .ForMember(dest => dest.ExamTitle, opt => opt.MapFrom(src => src.Exam.Title))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score ?? 0))
                .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => src.MaxScore ?? 0))
                .ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => src.Percentage ?? 0))
                .ForMember(dest => dest.PassingPercentage, opt => opt.Ignore()); // Set manually

            CreateMap<StudentAnswer, StudentAnswerDto>()
                .ForMember(dest => dest.ExamAttemtId, opt => opt.MapFrom(src => src.AttemptId))
                .ForMember(dest => dest.ChoiceAnswer, opt => opt.MapFrom(src => src.SelectedChoice != null ? src.SelectedChoice.Text : string.Empty));

            CreateMap<Question, QuestionToReturnForStudentDto>();


            // ExamAttempt mappings
            CreateMap<ExamAttempt, ExamAttemptResponseForStudentViewModel>();
                //.ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"))
                //.ForMember(dest => dest.ExamTitle, opt => opt.MapFrom(src => src.Exam.Title));

            // StudentAnswer mappings  
            CreateMap<StudentAnswer, AttemptAnswerResponseForStudentViewModel>();
        }
    }
}