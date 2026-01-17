using AutoMapper;
using Examination_System.Data;
using Examination_System.MappingProfiles;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Services;
using Examination_System.Services.CourseServices;
using Examination_System.Services.CourseServices.Repository;
using Examination_System.Services.CourseServices.Validator;
using Examination_System.Services.ExamAttemptServices;
using Examination_System.Services.ExamAttemptServices.Repositories;
using Examination_System.Services.ExamAttemptServices.Validators;
using Examination_System.Services.ExamServices;
using Examination_System.Services.ExamServices.Managers;
using Examination_System.Services.ExamServices.Repositories;
using Examination_System.Services.ExamServices.Validators;
using Examination_System.Services.QuestionServices;
using Examination_System.Services.QuestionServices.Repositories;
using Examination_System.Services.StudentService;
using Examination_System.Services.StudentService.Repository;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Extensions
{
    public static class ApplicaionServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICourseServices, CourseServices>();
            services.AddScoped<IExamServices, Examination_System.Services.ExamServices.ExamServices>();
            services.AddScoped<IQuestionServices, QuestionServices>();
            services.AddScoped<IStudentServices, StudentServices>();
            services.AddScoped<IExamAttemptServices, ExamAttemptServices>();
            services.AddScoped<IExamAttemptRepository, ExamAttemptRepository>();
            services.AddScoped<IExamAvailabilityValidator, ExamAvailabilityValidator>();
            services.AddScoped<IExamSubmissionValidator, ExamSubmissionValidator>();
            services.AddScoped<IExamEvaluator, ExamEvaluator>();
            services.AddScoped<ExamAuthorizationValidator>();
            services.AddScoped<ExamQuestionManager>();
            services.AddScoped<IExamRepository, ExamRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<CourseValidator>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Program>();

            // AutoMapper - scans assembly for all Profile classes
            services.AddAutoMapper(typeof(ApplicationMappingProfile).Assembly);

            services.AddDbContext<Context>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}
