using Core.Entities;
using Core.Repositories;
using Infrastructure.Data;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public abstract class GenericRepository<T> : IGenericRepository<T> where T : class, IEntityBase
{
    private readonly IMongoCollection<T> _dbSet;
    private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;

    protected GenericRepository(IMongoDbContext mongoContext)
    {
        _dbSet = mongoContext.GetCollection<T>(typeof(T).Name);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.Find(_filterBuilder.Empty).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filterExpression)
    {
        return await _dbSet.Find(filterExpression).ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        var filter = _filterBuilder.Eq("_id", id);
        return await _dbSet.Find(filter).FirstOrDefaultAsync(); ;
    }

    public async Task<T> GetByIdAsync(Guid id, Expression<Func<T, bool>> filterExpression)
    {
        var filter = _filterBuilder.Eq("_id", id) & filterExpression;
        return await _dbSet.Find(filter).SingleOrDefaultAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        var filter = _filterBuilder.Eq("_id", entity.Id);
        await _dbSet.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = _filterBuilder.Eq("_id", id);
        await _dbSet.DeleteOneAsync(filter);
    }
}