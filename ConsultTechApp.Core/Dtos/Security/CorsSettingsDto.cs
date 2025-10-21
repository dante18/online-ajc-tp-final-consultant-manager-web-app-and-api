namespace ConsultTechApp.Core.Dtos.Security;

public sealed class CorsSettingsDto
{
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; init; } = ["http://localhost:3000", "https://localhost:3001"];

    public bool AllowCredentials { get; init; } = true;
}
