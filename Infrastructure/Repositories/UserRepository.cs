using Core.Entities.Models;
using Core.Repositories;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> usersCollection;
    private readonly FilterDefinitionBuilder<User> filterBuilder = Builders<User>.Filter;

    public UserRepository(IMongoDbContext context)
    {
        usersCollection = context.Users;
    }

    public async Task CreateUserAsync(User user)
    {
        await usersCollection.InsertOneAsync(user);
    }

    public async Task DeleteUserAsync(System.Guid id)
    {
        var filter = filterBuilder.Eq(user => user.Id, id);

        await usersCollection.DeleteOneAsync(filter);
    }

    public async Task<User> GetUserAsync(System.Guid id)
    {
        var filter = filterBuilder.Eq(user => user.Id, id);

        return await usersCollection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await usersCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        var filter = filterBuilder.Eq(existingUser => existingUser.Id, user.Id);

        await usersCollection.ReplaceOneAsync(filter, user);
    }
}