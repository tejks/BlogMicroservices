using Core.Entities.Models;

namespace Core.Repositories;

public interface IPostRepository : IGenericRepository<Post>
{
    IEnumerable<Post> GetCommentsByUserIdSync(Guid userId);
}