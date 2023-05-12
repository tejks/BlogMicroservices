using Core.Entities.Models;

namespace Core.Repositories;

public interface IUserRepository
{
    Task CreateUserAsync(User user);
    Task DeleteUserAsync(System.Guid id);
    Task<User> GetUserAsync(System.Guid id);
    Task<IEnumerable<User>> GetUsersAsync();
    Task UpdateUserAsync(User user);
}