using AuthAPI.Dto;
using Core.Entities.Models;
using Core.Repositories;

namespace AuthAPI.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    
    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var usersDto = users.Select(UserDto.UserToDto);

        return usersDto;
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : UserDto.UserToDto(user);
    }
    
    public async Task<UserDto> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        return user is null ? null : UserDto.UserToDto(user);
    }

    public async Task<UserDto> CreateAsync(UserDto entity)
    {
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Email = entity.Email,
            Password = entity.Password,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await _userRepository.CreateAsync(user);
        var newUser = await _userRepository.GetByIdAsync(user.Id);

        return UserDto.UserToDto(newUser);
    }

    public async Task<UserDto> CreateAsync(UserCreateDto entity)
    {
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Email = entity.Email,
            Password = entity.Password,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            CreatedDate = DateTimeOffset.UtcNow
        };
        
        await _userRepository.CreateAsync(user);
        var newUser = await _userRepository.GetByIdAsync(user.Id);

        return UserDto.UserToDto(newUser);
    }

    public async Task UpdateAsync(UserDto entity)
    {
        var user = new User()
        {
            Id = entity.Id,
            Email = entity.Email,
            Password = entity.Password,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            CreatedDate = entity.CreatedDate
        };

        await _userRepository.UpdateAsync(user);
    }

    public async Task UpdateAsync(Guid id, UserCreateDto entity)
    {
        var oldUserData = await GetByIdAsync(id);
        
        var user = new User()
        {
            Id = oldUserData.Id,
            Email = entity.Email,
            Password = entity.Password,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            CreatedDate = oldUserData.CreatedDate
        };

        await _userRepository.UpdateAsync(user);
    }
    public async Task DeleteAsync(Guid id)
    {
        await _userRepository.DeleteAsync(id);
    }
}