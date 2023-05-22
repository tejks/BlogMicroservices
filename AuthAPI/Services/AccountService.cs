using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        private readonly ITokenRepository _tokenRepository;

        public AccountService(IJwtAuthService jwtAuthService, IUserRepository userRepository, IPasswordService passwordService, IUserService userService, ITokenRepository tokenRepository)
        {
            _jwtAuthService = jwtAuthService;
            _userRepository = userRepository;
            _passwordService = passwordService;
            _userService = userService;
            _tokenRepository = tokenRepository;
        }

        public async Task<UserAuthResponse> Login(UserLoginDto userLoginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(userLoginDto.Email);
            var authModel = new UserAuthResponse() { IsAuthenticated = false };
            
            if (user == null)
            {
                authModel.Message = $"Account with {userLoginDto.Email} doesn't exist.";
                return authModel;
            };

            if (!Verify(userLoginDto.Password, user))
            {
                authModel.Message = $"Incorrect credentials.";
                return authModel;
            };

            var token = _jwtAuthService.GenerateAccessToken(user);

            authModel.IsAuthenticated = true;
            authModel.Message = "Logged in.";
            authModel.Token = token;
            authModel.Email = user.Email;
            authModel.Role = user.Role;

            if (user.RefreshTokens.Any(a => a.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(a => a.IsActive)!;
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.Expires;
            }
            else
            {
                var refreshToken = _jwtAuthService.GenerateRefreshToken(user);
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.Expires;

                var newToken = await _tokenRepository.AddTokenAsync(refreshToken);
                user.RefreshTokens.Add(newToken); 
            }
            
            return authModel;
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
        
        public async Task<UserAuthResponse> RefreshToken(TokenRequestModel tokenRequestModel)
        {
            var authModel = new UserAuthResponse() { IsAuthenticated = false };

            var refreshToken = await _tokenRepository.GetByTokenAsync(tokenRequestModel.RefreshToken);

            if (refreshToken is null)
            {
                authModel.Message = $"The specified token does not exist.";
                return authModel;
            }
            var user = await _userRepository.GetByIdAsync(refreshToken.UserId);

            if (user is null)
            {
                authModel.Message = $"Invalid client request.";
                return authModel;
            }

            if (!refreshToken.IsActive)
            {
                authModel.Message = $"Token expired.";
                return authModel;
            }
            
            //Revoke current refresh token
            refreshToken.RevokedDate = DateTime.UtcNow;
            await _tokenRepository.UpdateTokenAsync(refreshToken);
            var newRefreshToken = _jwtAuthService.GenerateRefreshToken(user);
            
            var newToken = await _tokenRepository.AddTokenAsync(newRefreshToken);
            user.RefreshTokens.Add(newToken);
            
            // Generate new jwt
            authModel.IsAuthenticated = true;
            authModel.Token = _jwtAuthService.GenerateAccessToken(user);
            authModel.Email = user.Email;
            authModel.Role = user.Role;
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.Expires;
            
            return authModel;
        }

        public async Task<bool> RevokeToken(TokenRequestModel tokenRequestModel)
        {
            var refreshToken = await _tokenRepository.GetByTokenAsync(tokenRequestModel.RefreshToken);
            
            if (refreshToken is null) return false;
            if (!refreshToken.IsActive) return false;
            
            refreshToken.RevokedDate = DateTime.UtcNow;
            await _tokenRepository.UpdateTokenAsync(refreshToken);
            
            return true;
        }
    }
}
