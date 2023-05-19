using AuthAPI.Models;

namespace Core.Repositories;

public interface ITokenRepository
{
    Task<RefreshToken> GetByTokenAsync(string token);
    Task<RefreshToken> GetUserTokenAsync(Guid id);
    Task<RefreshToken> AddTokenAsync(RefreshToken token);
    Task UpdateTokenAsync(RefreshToken token);
}