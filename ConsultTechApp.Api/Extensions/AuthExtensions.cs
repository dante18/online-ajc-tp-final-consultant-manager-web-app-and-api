using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ConsultTechApp.Api.Extensions;

public static class AuthExtensions
{
    public static void AddJwtAuthentication(this IHostApplicationBuilder builder)
    {
        // Configure authentication schemes with JWT Bearer as default
        _ = builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
               // Configure JWT Bearer token validation
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new()
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
                       ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"),
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue("Jwt:Key", string.Empty))),
                   };
               });
    }
}
