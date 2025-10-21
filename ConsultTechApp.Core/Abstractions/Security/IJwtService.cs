using ConsultTechApp.Core.Entities;

namespace ConsultTechApp.Core.Abstractions.Security;

public interface IJwtService
{
    string GenerateToken(User user);
}
