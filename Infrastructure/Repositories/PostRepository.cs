using Core.Entities.Models;
using Core.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class PostRepository : GenericRepository<Post>, IPostRepository
{
    public PostRepository(IMongoDbContext mongoContext) : base(mongoContext)
    {
    }
}