using Core.Entities;
using Core.Entities.Models;

namespace AuthAPI.Dto;

public class UserDto : UserCreateDto, IEntityBase
{
    public Guid Id { get; init; }
    public DateTimeOffset CreatedDate { get; init; }

    public static UserDto UserToDto(User user)
    {
        return new UserDto()
        {
            Id = user.Id,
            Email = user.Email,
            Password = user.Password,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }
}