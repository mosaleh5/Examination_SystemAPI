using Examination_System.Data;
using Examination_System.Helpers;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Services;
using Examination_System.Services.CourseServices;
using Examination_System.Services.ExamAttemptServices;
using Examination_System.Services.ExamServices;
using Examination_System.Services.QuestionServices;
using Examination_System.Services.StudentService;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using FluentValidation;

namespace Examination_System.Extensions
{
    public static class ApplicaionServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICourseServices , CourseServices>();
            services.AddScoped<IExamServices , ExamServices>();
            services.AddScoped<IQuestionServices , QuestionServices>();
            services.AddScoped<IStudentServices , StudentServices>();
            services.AddScoped<IExamAttemptServices , ExamAttemptServices>();

          
            services.AddValidatorsFromAssemblyContaining<Program>();

            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddDbContext<Context>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
            );
        /*    Services.AddScoped<DatabaseSeeder>();
            Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler =
                         System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });*/

            return services;
        }

    }
}
