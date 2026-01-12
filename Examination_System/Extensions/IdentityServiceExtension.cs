using Examination_System.Data;
using Examination_System.Models;
using Microsoft.AspNetCore.Identity;

namespace Examination_System.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<Context>()
            .AddDefaultTokenProviders();

            // JWT Configuration
            services.AddAuthentication(options =>
            {
                // ... JWT setup
            });

            return services;
        }
    }
}