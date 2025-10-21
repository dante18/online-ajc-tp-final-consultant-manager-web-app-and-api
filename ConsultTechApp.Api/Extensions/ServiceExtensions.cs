using ConsultTechApp.Core.Abstractions.Security;
using ConsultTechApp.Core.Context;
using ConsultTechApp.Core.Dtos.Security;
using ConsultTechApp.Core.Dtos.Settings;
using ConsultTechApp.Core.Entities;
using ConsultTechApp.Core.Extensions;
using ConsultTechApp.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

namespace ConsultTechApp.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationCoreService(this IServiceCollection services, IConfiguration config)
    {
        // Bind configuration models to strongly-typed settings classes
        _ = services.Configure<JwtSettingsDto>(config.GetSection(JwtSettingsDto.SectionName));
        _ = services.Configure<CorsSettingsDto>(config.GetSection(CorsSettingsDto.SectionName));

        // Database configuration
        var connectionString = config.GetConnectionString("Data")
                               ?? throw new InvalidOperationException("Connection string 'Data' not found.");

        _ = services.AddDbContext<ApplicationStoreContext>(options => options.UseSqlServer(connectionString));

        // Email configuration
        services.Configure<EmailSettings>(
            config.GetSection("EmailSettings"));

        // Load service since Core
        _ = services.AddConsultTechAppCore(config);

        return services;
    }

    public static IServiceCollection AddApplicationApiService(this IServiceCollection services)
    {
        _ = services.AddControllers(static options => options.SuppressAsyncSuffixInActionNames = false);

        // OpenAPI configuration for API documentation
        // Enables automatic API documentation generation and integration with Scalar
        _ = services.AddOpenApi();
        _ = services.AddEndpointsApiExplorer();

        // Swagger
        _ = services.AddSwaggerGen();

        return services;
    }

    public static IServiceCollection AddApplicationApiSecurityServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Authorization services
        _ = services.AddAuthorization();

        // CORS configuration using strongly-typed settings
        var corsSettings = configuration.GetSection(CorsSettingsDto.SectionName).Get<CorsSettingsDto>() ?? new CorsSettingsDto();

        // Configure Cross-Origin Resource Sharing (CORS) policy
        // Référence MS Doc: https://learn.microsoft.com/en-us/aspnet/core/security/cors
        _ = services.AddCors(options => options.AddDefaultPolicy(policy =>
        {
            _ = policy.WithOrigins(corsSettings.AllowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();

            if (corsSettings.AllowCredentials)
            {
                _ = policy.AllowCredentials();
            }
        }));

        // Configuration ASP.NET Core Identity
        _ = services.AddIdentity<User, IdentityRole<Guid>>(static options =>
            {
                // Configuration simplifiée pour la formation
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                // Configuration utilisateur
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ApplicationStoreContext>()
            .AddDefaultTokenProviders();

        // Enregistrement du service JWT pour Identity
        _ = services.AddScoped<IJwtService, JwtService>();

        return services;
    }

    public static IServiceCollection AddApplicationApiInfrastructureServices(this IServiceCollection services)
    {
        // Response caching services
        _ = services.AddResponseCaching();

        // Response compression configuration
        _ = services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            // Add Gzip compression provider (widely supported fallback)
            options.Providers.Add<GzipCompressionProvider>();
        });

        return services;
    }
}
