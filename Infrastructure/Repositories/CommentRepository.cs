using Core.Entities.Models;
using Core.Repositories;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly IMongoCollection<Comment> commentsCollection;
    private readonly FilterDefinitionBuilder<Comment> filterBuilder = Builders<Comment>.Filter;

    public CommentRepository(IMongoDbContext context)
    {
        commentsCollection = context.Comments;
    }

    public async Task CreateCommentAsync(Comment comment)
    {
        await commentsCollection.InsertOneAsync(comment);
    }

    public async Task DeleteCommentAsync(Guid id)
    {
        var filter = filterBuilder.Eq(comment => comment.Id, id);

        await commentsCollection.DeleteOneAsync(filter);
    }

    public async Task<Comment> GetCommentAsync(Guid id)
    {
        var filter = filterBuilder.Eq(comment => comment.Id, id);

        return await commentsCollection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<Comment>> GetCommentsAsync()
    {
        return await commentsCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task UpdateCommentAsync(Comment comment)
    {
        var filter = filterBuilder.Eq(existingComment => existingComment.Id, comment.Id);

        await commentsCollection.ReplaceOneAsync(filter, comment);
    }
}