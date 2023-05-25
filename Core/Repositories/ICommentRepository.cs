using Core.Entities.Models;

namespace Core.Repositories;

public interface ICommentRepository: IGenericRepository<Comment>
{
    IEnumerable<Comment> GetCommentsByPostIdSync(Guid postId);
    void DeleteCommentSync(Guid id);
}