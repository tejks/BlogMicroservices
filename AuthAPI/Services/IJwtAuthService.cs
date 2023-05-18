using System.Security.Claims;
using AuthAPI.Dto;
using AuthAPI.Models;
using Core.Entities.Models;
using Microsoft.AspNetCore.Authentication;

namespace AuthAPI.Services;

public interface IJwtAuthService
{
    Task<string> GenerateToken(LoginUser user);
    Task<AuthTokenResponse> Login(LoginUser loginUser);
    Task<UserDto> Register(UserCreateDto userCreateDto);
}