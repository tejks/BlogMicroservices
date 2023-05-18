using AuthAPI.Dto;
using AuthAPI.Models;
using Core.Entities.Models;
using Core.Repositories;
using Core.Services;

namespace AuthAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly IJwtAuthService _jwtAuthService;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUserService _userService;

        public AccountService(IJwtAuthService jwtAuthService, IUserRepository userRepository, IPasswordService passwordService, IUserService userService)
        {
            _jwtAuthService = jwtAuthService;
            _userRepository = userRepository;
            _passwordService = passwordService;
            _userService = userService;
        }

        public async Task<AuthTokenResponse?> Login(LoginUser loginUser)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginUser.Email);

            if (user == null) return null;

            if (!Verify(loginUser.Password, user)) return null;

            var token = _jwtAuthService.GenerateToken(user);

            return new AuthTokenResponse()
            {
                Token = token,
                UserId = user.Id,
            };
        }

        public async Task<UserDto?> Register(UserCreateDto userCreateDto)
        {
            var emailCheck = await _userRepository.GetUserByEmailAsync(userCreateDto.Email);

            if (emailCheck is not null) return null;

            var result = await _userService.CreateAsync(userCreateDto);

            return result;
        }

        public async Task<UserDto?> ChangePassword(Guid id, UserChangePasswordDto userChangePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null) return null;

            if (!Verify(userChangePasswordDto.OldPassword, user)) return null;

            var result = await _userService.ChangePassword(id, userChangePasswordDto);

            return result;
        }

        private bool Verify(string password, User user)
        {
            return _passwordService.VerifyPassword(password, user.PasswordHash, Convert.FromHexString(user.PasswordSalt));
        }
    }
}
