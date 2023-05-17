using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthAPI.Dto;
using AuthAPI.Models;
using Core.Entities.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AuthAPI.Services;

public class JwtAuthService : IJwtAuthService
{
    private readonly IConfiguration _config;
    private readonly IUserService _userService;
    private UserDto _user;

    public JwtAuthService(IConfiguration config, IUserService userService)
    {
        _config = config;
        _userService = userService;
    }

    public async Task<AuthTokenResponse> Login(LoginUser loginUser)
    {
        _user = await _userService.GetUserByEmailAsync(loginUser.Email);
            
        if(_user == null)
        {
            return null;
        }

        if (loginUser.Email != _user.Email || loginUser.Password != _user.Password)
        {
            return null;
        }

        var token = await GenerateToken(loginUser);

        return new AuthTokenResponse()
        {
            Token = token,
            UserId = _user.Id,
        };
    }
    
    public async Task<UserDto> Register(UserCreateDto userCreateDto)
    {
        var emailCheck = await _userService.GetUserByEmailAsync(userCreateDto.Email);
        if (emailCheck is not null)
        {
            return null;
        }
        
        var result = await _userService.CreateAsync(userCreateDto);

        return result;
    }
    
    public Task<string> GenerateToken(LoginUser loginUser)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expirationTimeStamp = DateTime.Now.AddMinutes(15);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, _user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: expirationTimeStamp,
            signingCredentials: credentials
        );
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        
        return Task.FromResult(tokenString);
    }
}