using Examination_System.Data;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Extensions
{
    public static class ApplicaionServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {

            Services.AddDbContext<Context>(option =>
            {
                option.UseSqlServer();
            }
);
            Services.AddScoped<DatabaseSeeder>();
            Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler =
                         System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            return Services;
        }
    }
}
