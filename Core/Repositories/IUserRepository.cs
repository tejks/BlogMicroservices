using Core.Entities.Models;

namespace Core.Repositories;

public interface IUserRepository: IGenericRepository<User>
{
    Task<User> GetUserByEmailAsync(string email);
}