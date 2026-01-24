using Examination_System.Data;
using Examination_System.Data.SeedData;
using Examination_System.Extensions;
using Examination_System.Filters.ValidationFilters;
using Examination_System.Middleware;
using Examination_System.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<UnifiedValidationFilter>();  

}).AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddScoped<TransactionMiddleware>();
builder.Services.AddScoped<JsonSeedingService>();
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
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<JsonSeedingService>>();
        await context.Database.MigrateAsync();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        logger.LogInformation("Starting database seeding...");
        await UserDataSeeding.UserSeedingAsync(userManager, roleManager);
        var jsonSeedingService = scope.ServiceProvider.GetRequiredService<JsonSeedingService>();
        await jsonSeedingService.SeedFromJsonAsync(context, userManager, roleManager, logger);

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


