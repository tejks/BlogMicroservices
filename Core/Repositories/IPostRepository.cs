using Core.Entities.Models;

namespace Core.Repositories;

public interface IPostRepository
{
    Task CreatePostAsync(Post post);
    Task DeletePostAsync(Guid id);
    Task<Post> GetPostAsync(Guid id);
    Task<IEnumerable<Post>> GetPostsAsync();
    Task UpdatePostAsync(Post post);
}