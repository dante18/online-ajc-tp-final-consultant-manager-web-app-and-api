using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsultTechApp.Web.Services.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddConsultTechAppService(this IServiceCollection services, IConfiguration config)
    {
        var uriApi = config.GetValue<string>("ApiSettings:BaseUrl");

        return services;
    }
}
