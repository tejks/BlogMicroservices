using PostsAPI.Dto.Post;

namespace PostsAPI.Services;

public interface IPostService
{
    Task<IEnumerable<PostDto>> GetAllByUserIdAsync(Guid userId);
    Task<PostDto> GetUserLastPostAsync(Guid userId);
    Task<IEnumerable<PostDto>> GetAllAsync();
    Task<PostDto> GetByIdAsync(Guid postId);
    Task<PostDto> CreateAsync(Guid userId, PostCreateDto entity);
    Task UpdateAsync(Guid postId, PostUpdateDto entity); 
    Task DeleteAsync(Guid id);
}