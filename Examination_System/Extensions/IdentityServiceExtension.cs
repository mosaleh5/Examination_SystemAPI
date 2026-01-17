using Examination_System.Data;
using Examination_System.Models;
using Examination_System.Services.CurrentUserServices;
using Examination_System.Services.TokenServices;
using Examination_System.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Examination_System.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
           
            services.AddScoped<ITokenServices, TokenServices>();
            services.AddScoped<ICurrentUserServices, CurrentUserServices>();
            services.AddScoped<IUserServices, UserServices>();
            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            { 
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;     
                options.User.RequireUniqueEmail = true;
            })
           .AddEntityFrameworkStores<Context>()
           .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection("JwtSettings");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,//sourse of token 
                    ValidateAudience = true,//is this token for this app 
                    ValidateLifetime = true, //is live or expired
                    ValidateIssuerSigningKey = true, //check key if original 
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                   Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
                    RoleClaimType = ClaimTypes.Role, // Critical for role-based auth
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

            return services;
        }
    }
}