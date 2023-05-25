using Core.Entities.Models;
using Core.Repositories;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class PostRepository : GenericRepository<Post>, IPostRepository
{
    private readonly FilterDefinitionBuilder<Post> _filterBuilder = Builders<Post>.Filter;
    private readonly IMongoCollection<Post> _collection;
    
    public PostRepository(IMongoDbContext mongoContext) : base(mongoContext)
    {
        _collection = mongoContext.Posts;
    }
    
    public IEnumerable<Post> GetCommentsByUserIdSync(Guid userId)
    {
        var filter = _filterBuilder.Eq(x => x.UserId, userId);
        return _collection.Find(filter).ToList();
    }
}