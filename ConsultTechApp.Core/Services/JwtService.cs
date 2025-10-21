using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ConsultTechApp.Core.Abstractions.Security;
using ConsultTechApp.Core.Dtos.Security;
using ConsultTechApp.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ConsultTechApp.Core.Services;

public class JwtService : IJwtService
{
    private readonly ILogger<JwtService> logger;
    private readonly JwtSettingsDto jwtSettings;
    private readonly UserManager<User> userManager;

    public JwtService(ILogger<JwtService> logger, IOptions<JwtSettingsDto> jwtOptions, UserManager<User> userManager)
    {
        this.logger = logger;
        jwtSettings = jwtOptions.Value;
        this.userManager = userManager;
    }

    public string GenerateToken(User user)
    {
        var userRoles = userManager.GetRolesAsync(user).Result;

        // Create JWT claims
        List<Claim> claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(ClaimTypes.Name, user.Email),
        };

        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        logger.LogDebug("Created JWT claims for user: {Username}", user.Email);

        // Retrieve JWT configuration parameters
        var key = jwtSettings.Key;

        if (string.IsNullOrWhiteSpace(key))
        {
            logger.LogError("JWT Key is not configured or empty");
            throw new InvalidOperationException("JWT Key is not configured.");
        }

        var issuer = jwtSettings.Issuer
                  ?? throw new InvalidOperationException("The JWT issuer is not configured.");

        var audience = jwtSettings.Audience
                    ?? throw new InvalidOperationException("The JWT audience is not configured.");

        // Get token expiration time from configuration (default to 30 minutes)
        var expirationMinutes = jwtSettings.ExpirationInMinutes;

        logger.LogDebug("JWT configuration loaded - Issuer: {Issuer}, Audience: {Audience}, Expiration: {ExpirationMinutes} minutes",
            issuer,
            audience,
            expirationMinutes);

        // Create signing credentials using HMAC SHA-256
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create the JWT token
        var tokenExpiry = DateTime.UtcNow.AddMinutes(expirationMinutes);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: tokenExpiry,
            signingCredentials: creds);

        // Serialize the token to string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}