﻿namespace Core.Entities.Models;

public record Comment : IEntityBase
{
    public Guid Id { get; init; }
    public string Text { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
    public Guid PostId { get; init; }
    public Guid UserId { get; init; }

    public Post Post { get; init; }
    public User User { get; init; }
}