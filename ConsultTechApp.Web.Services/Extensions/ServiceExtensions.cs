using ConsultTechApp.Web.Services.Abstractions.Application;
using ConsultTechApp.Web.Services.Services.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsultTechApp.Web.Services.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddConsultTechAppService(this IServiceCollection services, IConfiguration config)
    {
        var uriApi = config.GetValue<string>("ApiSettings:BaseUrl");

        _ = services.AddHttpClient("Customers", client => client.BaseAddress = new($"{uriApi}/api/customers/"));

        _ = services.AddScoped<IApiCustomersService, CustomersService>();

        return services;
    }
}
