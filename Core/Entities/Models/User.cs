using AuthAPI.Models;
using Core.Enums;

namespace Core.Entities.Models;

public record User : IEntityBase
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public Role Role { get; set; }
    public DateTimeOffset CreatedDate { get; init; }
    
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}