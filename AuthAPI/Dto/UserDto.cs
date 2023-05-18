using Core.Entities.Models;

namespace AuthAPI.Dto;

public class UserDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public DateTimeOffset CreatedDate { get; init; }

    public static UserDto UserToDto(User user)
    {
        return new UserDto()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedDate = user.CreatedDate,
        };
    }
}