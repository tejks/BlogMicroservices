using System.Security.Claims;
using AuthAPI.Models;
using Core.Entities.Models;

namespace AuthAPI.Services;

public interface IJwtAuthService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(User user);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}