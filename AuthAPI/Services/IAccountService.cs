using AuthAPI.Dto;
using AuthAPI.Models;

namespace AuthAPI.Services
{
    public interface IAccountService
    {
        Task<AuthTokenResponse?> Login(LoginUser loginUser);
        Task<UserDto?> Register(UserCreateDto userCreateDto);
        Task<UserDto?> ChangePassword(Guid id, UserChangePasswordDto userChangePasswordDto);
    }
}
