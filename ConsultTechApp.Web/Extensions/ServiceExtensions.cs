using ConsultTechApp.Web.Services.Extensions;

namespace ConsultTechApp.Web.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddConsultTechAppWeb(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddConsultTechAppService(config);

        return services;
    }
}