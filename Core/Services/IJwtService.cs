using System.Security.Claims;
using AuthAPI.Models;
using Core.Entities.Models;

namespace Core.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(User user);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    bool Verify(string token);
}