using Core.Entities.Models;

namespace AuthAPI.Services;

public interface IJwtAuthService
{
    string GenerateToken(User user);
}