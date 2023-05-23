using Core.Entities.Models;
using Core.Repositories;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    public CommentRepository(IMongoDbContext context): base(context)
    {
    }
}