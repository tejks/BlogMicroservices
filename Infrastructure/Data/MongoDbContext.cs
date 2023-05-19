using AuthAPI.Models;
using Core.Entities.Models;
using MongoDB.Driver;

namespace Infrastructure.Data;

public class MongoDbContext : IMongoDbContext
{
    private IMongoDatabase Database { get; set; }

    private const string DatabaseName = "BlogDB";
    private const string PostCollectionName = "Posts";
    private const string CommentCollectionName = "Comments";
    private const string UserCollectionName = "Users";
    private const string RefreshTokenCollectionName = "RefreshTokens";

    public MongoDbContext(IMongoClient mongoClient)
    {
        Database = mongoClient.GetDatabase(DatabaseName);

        Posts = Database.GetCollection<Post>(PostCollectionName);
        Comments = Database.GetCollection<Comment>(CommentCollectionName);
        Users = Database.GetCollection<User>(UserCollectionName);
        RefreshTokens = Database.GetCollection<RefreshToken>(RefreshTokenCollectionName);
    }

    public IMongoCollection<Post> Posts { get; }
    public IMongoCollection<Comment> Comments { get; }
    public IMongoCollection<User> Users { get; }
    public IMongoCollection<RefreshToken> RefreshTokens { get; }
    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return Database.GetCollection<T>(name);
    }
}