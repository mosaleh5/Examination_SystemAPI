using Examination_System.Data;
using Examination_System.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Examination_System.Models;
using Examination_System.Data.SeedData;

using Examination_System.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddScoped<TransactionMiddleware>();
//builder.Services.AddScoped<GlobalExceptionHandlerMiddleware>();

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<TransactionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<Context>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();
        await context.Database.MigrateAsync();//update database to latest migration

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        // Fix: Use IdentityRole<Guid> to match Context.cs
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        logger.LogInformation("Starting database seeding...");
        await UserDataSeeding.UserSeedingAsync(userManager, roleManager);
        logger.LogInformation("User seeding completed");
        await DatabaseSeeder.SeedAsync(context, logger);
        logger.LogInformation("Database seeding completed");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database seeding");
        // Decide: throw to prevent startup, or continue without seeding
    }
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


