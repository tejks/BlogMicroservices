using AuthAPI.Models;
using Core.Entities.Models;
using MongoDB.Driver;

namespace Infrastructure.Data
{
    public interface IMongoDbContext
    {
        IMongoCollection<Comment> Comments { get; }
        IMongoCollection<Post> Posts { get; }
        IMongoCollection<User> Users { get; }
        IMongoCollection<RefreshToken> RefreshTokens { get; }
        IMongoCollection<T> GetCollection<T>(string name);
    }
}