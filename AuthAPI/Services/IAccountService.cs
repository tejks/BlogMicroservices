using AuthAPI.Dto;
using AuthAPI.Models;

namespace AuthAPI.Services
{
    public interface IAccountService
    {
        Task<UserAuthResponse> Login(UserLoginDto userLoginDto);
        Task<UserDto?> Register(UserCreateDto userCreateDto);
        Task<UserDto?> ChangePassword(Guid id, UserChangePasswordDto userChangePasswordDto);
        Task<UserAuthResponse> RefreshToken(TokenRequestModel tokenRequestModel);
        Task<bool> RevokeToken(TokenRequestModel tokenRequestModel);
    }
}
