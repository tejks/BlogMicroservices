using Core.Entities;
using Core.Repositories;
using Infrastructure.Data;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public abstract class GenericRepository<T> : IGenericRepository<T> where T : class, IEntityBase
{
    private readonly IMongoDbContext _mongoContext;
    private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;
    private readonly IMongoCollection<T> _collection;

    protected GenericRepository(IMongoDbContext mongoContext)
    {
        _mongoContext = mongoContext;
        // _collection = mongoContext.GetCollection<T>(typeof(T).Name); // Non-plural name
        _collection = SetupCollection();
    }

    private IMongoCollection<T> SetupCollection()
    {
        try
        {
            var pluralizedName = typeof(T).Name.EndsWith("s") ? typeof(T).Name : typeof(T).Name + "s";
            var collection = _mongoContext.GetCollection<T>(pluralizedName);
            return collection;
        }
        catch (MongoException ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_filterBuilder.Empty).ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        var filter = _filterBuilder.Eq("_id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
        var item = await GetByIdAsync(entity.Id);
        return item;
    }

    public async Task UpdateAsync(T entity)
    {
        var filter = _filterBuilder.Eq("_id", entity.Id);
        await _collection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = _filterBuilder.Eq("_id", id);
        await _collection.DeleteOneAsync(filter);
    }
}