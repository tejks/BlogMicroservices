using Core.Entities;
using System.Linq.Expressions;

namespace Core.Repositories;

public interface IGenericRepository<T> where T : class, IEntityBase
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filterExpression);
    Task<T> GetByIdAsync(Guid id);
    Task<T> GetByIdAsync(Guid id, Expression<Func<T, bool>> filterExpression);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}