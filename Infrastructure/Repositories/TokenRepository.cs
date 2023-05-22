using AuthAPI.Models;
using Core.Repositories;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Infrastructure.Repositories;

public class TokenRepository: ITokenRepository
{
    private readonly FilterDefinitionBuilder<RefreshToken> _filterBuilder = Builders<RefreshToken>.Filter;
    private readonly IMongoCollection<RefreshToken> _collection;
    
    public TokenRepository(IMongoDbContext mongoContext)
    {
        _collection = mongoContext.RefreshTokens;
    }

    public async Task<RefreshToken> GetByIdAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
    public async Task<RefreshToken> GetByTokenAsync(string token)
    {
        var filter = _filterBuilder.Eq(x => x.Token, token);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
    public async Task<RefreshToken> GetUserTokenAsync(Guid id)
    {
        var filter = _filterBuilder.Eq(x => x.UserId, id) & _filterBuilder.Eq(x => x.IsActive, true);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
    
    public async Task<RefreshToken> AddTokenAsync(RefreshToken token)
    {
        await _collection.InsertOneAsync(token);
        var item = await GetByIdAsync(token.Id);
        return item;
    }
    public async Task UpdateTokenAsync(RefreshToken entity)
    {
        var filter = _filterBuilder.Eq("_id", entity.Id);
        await _collection.ReplaceOneAsync(filter, entity);
    }
}