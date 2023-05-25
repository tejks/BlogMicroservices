using CommentsAPI.Dto.Comment;
using Core.Repositories;

namespace CommentsAPI.Services;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetAllByUserIdAsync(Guid userId);
    Task<IEnumerable<CommentDto>> GetAllByPostIdAsync(Guid postId);
    Task<IEnumerable<CommentDto>> GetAllAsync();
    Task<CommentDto> GetByIdAsync(Guid postId);
    Task<CommentDto> CreateAsync(Guid userId, CommentCreateDto entity);
    Task UpdateAsync(Guid postId, CommentUpdateDto entity); 
    Task DeleteAsync(Guid postId);
}