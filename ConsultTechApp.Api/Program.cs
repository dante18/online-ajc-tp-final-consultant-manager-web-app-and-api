using ConsultTechApp.Api.Extensions;
using ConsultTechApp.Core.Context;
using ConsultTechApp.Core.Entities;
using ConsultTechApp.Core.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container
builder.Services.AddApplicationCoreService(builder.Configuration);
builder.Services.AddApplicationApiService();
builder.Services.AddApplicationApiSecurityServices(builder.Configuration);
builder.Services.AddApplicationApiInfrastructureServices();

// Authentication (JWT)
builder.AddJwtAuthentication();

var app = builder.Build();

// Initialize database
using var scope = app.Services.CreateScope();

try
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationStoreContext>();
    await context.Database.MigrateAsync();

    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Database initialized successfully.");

    if (app.Environment.IsDevelopment())
    {
        await DatabaseApplicationSeeder.SeedDevDataAsync(scope.ServiceProvider);
    }
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while creating the database.");
    throw;
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Security headers
app.UseHttpsRedirection();

// CORS (before authentication)
app.UseCors();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Response compression (early in pipeline)
app.UseResponseCompression();

// Response caching
app.UseResponseCaching();

// API endpoints
app.MapControllers();

app.Run();