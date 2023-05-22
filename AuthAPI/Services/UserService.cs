using AuthAPI.Dto;
using Core.Entities.Models;
using Core.Enums;
using Core.Repositories;
using Core.Services;

namespace AuthAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    
    public UserService(IUserRepository userRepository, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var usersDto = users.Select(UserDto.UserToDto);

        return usersDto;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : UserDto.UserToDto(user);
    }
    
    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        return user is null ? null : UserDto.UserToDto(user);
    }

    public async Task<UserDto> CreateAsync(UserCreateDto entity)
    {
        var hash = _passwordService.HashPassword(entity.Password, out var salt);

        var user = new User()
        {
            Id = Guid.NewGuid(),
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Role = Role.User,
            PasswordHash = hash,
            PasswordSalt = Convert.ToHexString(salt),
            CreatedDate = DateTimeOffset.UtcNow
        };
        
        await _userRepository.CreateAsync(user);

        return UserDto.UserToDto(user);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UserUpdateDto entity)
    {
        var oldUserData = await _userRepository.GetByIdAsync(id);

        var user = new User()
        {
            Id = oldUserData.Id,
            Email = entity.Email,
            Role = oldUserData.Role,
            PasswordHash = oldUserData.PasswordHash,
            PasswordSalt = oldUserData.PasswordSalt,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            CreatedDate = oldUserData.CreatedDate
        };

        await _userRepository.UpdateAsync(user);

        return UserDto.UserToDto(user);
    }

    public async Task<UserDto> ChangePassword(Guid id, UserChangePasswordDto entity)
    {
        var oldUserData = await _userRepository.GetByIdAsync(id);

        var hash = _passwordService.HashPassword(entity.NewPassword, out var salt);

        var user = new User()
        {
            Id = oldUserData.Id,
            Email = oldUserData.Email,
            Role = oldUserData.Role,
            PasswordHash = hash,
            PasswordSalt = Convert.ToHexString(salt),
            FirstName = oldUserData.FirstName,
            LastName = oldUserData.LastName,
            CreatedDate = oldUserData.CreatedDate
        };

        await _userRepository.UpdateAsync(user);

        return UserDto.UserToDto(user);
    }

    public async Task<UserDto> ChangeRole(Guid id, Role role)
    {
        var oldUserData = await _userRepository.GetByIdAsync(id);
        
        var user = new User()
        {
            Id = oldUserData.Id,
            Email = oldUserData.Email,
            Role = role,
            PasswordHash = oldUserData.PasswordHash,
            PasswordSalt = oldUserData.PasswordSalt,
            FirstName = oldUserData.FirstName,
            LastName = oldUserData.LastName,
            CreatedDate = oldUserData.CreatedDate,
        };

        await _userRepository.UpdateAsync(user);

        return UserDto.UserToDto(user);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _userRepository.DeleteAsync(id);
    }
}