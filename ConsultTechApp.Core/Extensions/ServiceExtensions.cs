using ConsultTechApp.Core.Abstractions.Services;
using ConsultTechApp.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsultTechApp.Core.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddConsultTechAppCore(this IServiceCollection services, IConfiguration config)
    {
        // Email
        services.AddTransient<IEmailService, EmailService>();

        // Data services

        return services;
    }
}