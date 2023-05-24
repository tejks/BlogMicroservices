namespace Core.Entities.Models;

public record Post : IEntityBase
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Text { get; init; }
    public Guid UserId { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
}