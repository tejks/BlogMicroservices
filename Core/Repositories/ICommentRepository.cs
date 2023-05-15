﻿using Core.Entities.Models;

namespace Core.Repositories;

public interface ICommentRepository
{
    Task CreateCommentAsync(Comment comment);
    Task DeleteCommentAsync(Guid id);
    Task<Comment> GetCommentAsync(Guid id);
    Task<IEnumerable<Comment>> GetCommentsAsync();
    IEnumerable<Comment> GetCommentsSync();
    Task UpdateCommentAsync(Comment comment);
}