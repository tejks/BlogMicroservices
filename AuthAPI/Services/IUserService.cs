using AuthAPI.Dto;
using Core.Repositories;

namespace AuthAPI.Services;

public interface IUserService: IGenericRepository<UserDto>
{
    Task<UserDto> CreateAsync(UserCreateDto entity);
    Task UpdateAsync(Guid id, UserCreateDto entity);
    Task<UserDto> GetUserByEmailAsync(string email);
}