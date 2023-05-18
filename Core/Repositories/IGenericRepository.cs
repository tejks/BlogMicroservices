using Core.Entities;
using System.Linq.Expressions;

namespace Core.Repositories;

public interface IGenericRepository<T> where T : class, IEntityBase
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(Guid id);
    Task<T> CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}