namespace ConsultTechApp.Core.Dtos.Security;

public record AuthResponse
{
    public string Token { get; init; }

    public DateTime Expiration { get; init; }

    public string Email { get; init; }
}
