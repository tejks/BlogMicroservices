using Core.Entities.Models;
using Core.Repositories;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly FilterDefinitionBuilder<User> _filterBuilder = Builders<User>.Filter;
    private readonly IMongoCollection<User> _collection;
    public UserRepository(IMongoDbContext mongoContext) : base(mongoContext)
    {
        _collection = mongoContext.Users;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var filter = _filterBuilder.Eq(x => x.Email, email);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}