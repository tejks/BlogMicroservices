namespace Core.Entities.Models;

public record User : IEntityBase
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string Password { get; set; }
    public DateTimeOffset CreatedDate { get; init; }

    public IEnumerable<Comment> Comments { get; init; }
    public IEnumerable<Post> Posts { get; init; }
}