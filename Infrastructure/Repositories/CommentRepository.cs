using Core.Entities.Models;
using Core.Repositories;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    private readonly FilterDefinitionBuilder<Comment> _filterBuilder = Builders<Comment>.Filter;
    private readonly IMongoCollection<Comment> _collection;
    
    public CommentRepository(IMongoDbContext context): base(context)
    {
        _collection = context.Comments;
    }
    
    public IEnumerable<Comment> GetCommentsByPostIdSync(Guid postId)
    {
        var filter = _filterBuilder.Eq(x => x.PostId, postId);
        return _collection.Find(filter).ToList();
    }

    public void DeleteCommentSync(Guid id)
    {
        var filter = _filterBuilder.Eq("_id", id);
        _collection.DeleteOne(filter);
    }
}