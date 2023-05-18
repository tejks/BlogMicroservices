using AuthAPI.Dto;

namespace AuthAPI.Services;

public interface IUserService
{
    Task<UserDto> GetUserByEmailAsync(string email);
    Task<UserDto> CreateAsync(UserCreateDto entity);
    Task<UserDto> UpdateAsync(Guid id, UserUpdateDto entity);
    Task<UserDto> ChangePassword(Guid id, UserChangePasswordDto entity);
}