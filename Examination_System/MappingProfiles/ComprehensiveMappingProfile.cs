/*using AutoMapper;
using Examination_System.DTOs.Course;
using Examination_System.DTOs.Exam;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.DTOs.Question;
using Examination_System.DTOs.Student;
using Examination_System.Models;
using Examination_System.ViewModels.AttemptExam;
using Examination_System.ViewModels.Choice;
using Examination_System.ViewModels.Course;
using Examination_System.ViewModels.Exam;
using Examination_System.ViewModels.Question;

namespace Examination_System.MappingProfiles
{
    /// <summary>
    /// Comprehensive mapping profile for all entities in the system
    /// </summary>
    public class ComprehensiveMappingProfile : Profile
    {
        public ComprehensiveMappingProfile()
        {
            // ============================================
            // COURSE MAPPINGS
            // ============================================
            CreateMap<CreateCourseViewModel, CreateCourseDto>();

            CreateMap<Course, CourseDtoToReturn>();

            CreateMap<CreateCourseDto, Course>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.Exams, opt => opt.Ignore())
                .ForMember(dest => dest.CourseEnrollments, opt => opt.Ignore())
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<CourseDtoToReturn, CourseResponseViewModel>();

            CreateMap<UpdateCourseViewModel, UpdateCourseDto>();

            CreateMap<UpdateCourseDto, Course>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.InstructorId, opt => opt.Ignore())
                .ForMember(dest => dest.Exams, opt => opt.Ignore())
                .ForMember(dest => dest.CourseEnrollments, opt => opt.Ignore())
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<CourseEnrollment, CourseEnrollmentDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
                .ForMember(dest => dest.CourseDescription, opt => opt.MapFrom(src => src.Course.Description))
                .ForMember(dest => dest.InstructorId, opt => opt.MapFrom(src => src.Course.InstructorId))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src =>
                    src.Course.Instructor != null && src.Course.Instructor.User != null
                        ? src.Course.Instructor.User.FirstName + " " + src.Course.Instructor.User.LastName
                        : string.Empty));

            // ============================================
            // CHOICE MAPPINGS - CRITICAL ORDER!
            // ============================================
            CreateMap<ChoiceViewModel, ChoiceDto>();

            CreateMap<ChoiceDto, Choice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
                .ForMember(dest => dest.Question, opt => opt.Ignore())
                .ForMember(dest => dest.StudentAnswers, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<UpdateChoiceDto, Choice>()
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
                .ForMember(dest => dest.Question, opt => opt.Ignore())
                .ForMember(dest => dest.StudentAnswers, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<Choice, ChoiceDto>();

            CreateMap<Choice, ChoiceToReturnForInstructorDto>();

            CreateMap<Choice, ChoiceToReturnForStudentDto>();

            CreateMap<Choice, ChoiceToAttemptDto>();

            CreateMap<ChoiceToReturnForInstructorDto, ChoiceToReturnViewModel>();

            CreateMap<ChoiceToReturnForStudentDto, ChoiceToReturnViewModel>()
                .ForMember(dest => dest.IsCorrect, opt => opt.Ignore());

            CreateMap<ChoiceToAttemptDto, ChoiceForAttemptViewModel>();

            CreateMap<CreateChoiceViewModel, ChoiceDto>();

            CreateMap<UpdateChoiceViewModel, UpdateChoiceDto>();

            // ============================================
            // QUESTION MAPPINGS
            // ============================================
            CreateMap<CreateQuestionViewModel, CreateQuestionDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));

            CreateMap<CreateQuestionDto, Question>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices))
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForMember(dest => dest.ExamQuestions, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<UpdateQuestionViewModel, UpdateQuestionDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));

            CreateMap<UpdateQuestionDto, Question>()
                .ForMember(dest => dest.Choices, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.InstructorId, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForMember(dest => dest.ExamQuestions, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<Question, QuestionToReturnDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));

            CreateMap<Question, QuestionToReturnForStudentDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));

            CreateMap<Question, QuestionToAttemptDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));

            CreateMap<QuestionToReturnDto, QuestionToReturnViewModel>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));

            CreateMap<QuestionToReturnForStudentDto, QuestionToReturnViewModel>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));

            CreateMap<QuestionToAttemptDto, QuestionForAttemptViewModel>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));

            // ============================================
            // EXAM MAPPINGS
            // ============================================
            CreateMap<CreateExamViewModel, CreateExamDto>();

            CreateMap<CreateAutomaticExamViewModel, CreateAutomaticExamDto>();

            CreateMap<AssignStudentToExamDto, ExamAssignment>();

            CreateMap<CreateExamDto, Exam>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.ExamAttempts, opt => opt.Ignore())
                .ForMember(dest => dest.StudentExams, opt => opt.Ignore())
                .ForMember(dest => dest.ExamQuestions, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsAutomatic, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<CreateAutomaticExamDto, Exam>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.ExamAttempts, opt => opt.Ignore())
                .ForMember(dest => dest.StudentExams, opt => opt.Ignore())
                .ForMember(dest => dest.ExamQuestions, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsAutomatic, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<Exam, ExamToReturnDto>()
                .ForMember(dest => dest.ExamQuestions,
                    opt => opt.MapFrom(src => src.ExamQuestions.Select(eq => eq.Question)))
                .ForMember(dest => dest.CourseName,
                    opt => opt.MapFrom(src => src.Course.Name))
                .ForMember(dest => dest.InstructorName,
                    opt => opt.MapFrom(src => src.Instructor.User.FirstName + " " + src.Instructor.User.LastName));

            CreateMap<Exam, ExamToAttemptDto>()
                .ForMember(dest => dest.ExamId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExamQuestions,
                    opt => opt.MapFrom(src => src.ExamQuestions.Select(eq => eq.Question)))
                .ForMember(dest => dest.InstructorName,
                    opt => opt.MapFrom(src => src.Instructor.User.FirstName + " " + src.Instructor.User.LastName))
                .ForMember(dest => dest.ID, opt => opt.Ignore())
                .ForMember(dest => dest.StartedAt, opt => opt.Ignore());

            CreateMap<ExamQuestion, ExamQuestionToReturnDto>();

            CreateMap<ExamToReturnDto, ExamResponseViewModel>();

            CreateMap<ExamToReturnDto, ExamDetailedResponseViewModel>()
                .IncludeBase<ExamToReturnDto, ExamResponseViewModel>();

            CreateMap<ExamToAttemptDto, ExamToAttemptDetailedResponseForStudentViewModel>();

            // ============================================
            // EXAM ATTEMPT MAPPINGS
            // ============================================
            CreateMap<ExamAttempt, ExamAttemptDto>()
                .ForMember(dest => dest.StudentAnswers, opt => opt.MapFrom(src => src.StudentAnswers))
                .ForMember(dest => dest.ExamTitle, opt => opt.MapFrom(src => src.Exam.Title))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src =>
                    src.Student != null && src.Student.User != null
                        ? src.Student.User.FirstName + " " + src.Student.User.LastName
                        : string.Empty))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score ?? 0))
                .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => src.MaxScore ?? 0))
                .ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => src.Percentage ?? 0))
                .ForMember(dest => dest.PassingPercentage, opt => opt.MapFrom(src =>
                    src.Exam != null ? src.Exam.PassingPercentage : 0))
                .ForMember(dest => dest.IsSucceed, opt => opt.MapFrom(src =>
                    src.Percentage >= (src.Exam != null ? src.Exam.PassingPercentage : 0)));

            CreateMap<StudentAnswer, AttemptAnswersDto>()
                .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question))
                .ForMember(dest => dest.SelectedChoiceText, opt => opt.MapFrom(src =>
                    src.SelectedChoice != null ? src.SelectedChoice.Text : null));

            CreateMap<StudentAnswer, StudentAnswerToReturnDto>()
                .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question))
                .ForMember(dest => dest.SelectedChoiceText, opt => opt.MapFrom(src =>
                    src.SelectedChoice != null ? src.SelectedChoice.Text : string.Empty));

            CreateMap<StudentAnswer, StudentAnswerDto>()
                .ForMember(dest => dest.ExamAttemtId, opt => opt.MapFrom(src => src.AttemptId))
                .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question))
                .ForMember(dest => dest.ChoiceAnswer, opt => opt.MapFrom(src =>
                    src.SelectedChoice != null ? src.SelectedChoice.Text : string.Empty))
                .ForMember(dest => dest.IsCorrect, opt => opt.MapFrom(src => src.IsCorrect));

            CreateMap<AttemptAnswersDto, StudentAnswerToReturnDto>()
                .ForMember(dest => dest.SelectedChoiceText, opt => opt.MapFrom(src => src.SelectedChoiceText));

            CreateMap<ExamAttemptDto, ExamAttemptResponseForStudentViewModel>()
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.StudentAnswers));

            CreateMap<AttemptAnswersDto, AttemptAnswerResponseForStudentViewModel>()
                .ForMember(dest => dest.ChoiceAnswer, opt => opt.MapFrom(src => src.SelectedChoiceText));

            CreateMap<StudentAnswerDto, AttemptAnswerResponseForStudentViewModel>()
                .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question));

            CreateMap<StudentAnswerToReturnDto, AttemptAnswerResponseForStudentViewModel>()
                .ForMember(dest => dest.ChoiceAnswer, opt => opt.MapFrom(src => src.SelectedChoiceText));

            CreateMap<SubmitAnswerForStudentViewModel, SubmitAnswerDto>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId))
                .ForMember(dest => dest.choiceId, opt => opt.MapFrom(src => src.AnswerId));
        }
    }
}*/