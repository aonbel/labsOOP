using Domain.Entities.Core;

namespace Infrastructure.Interfaces;

public interface IRepository<T>
{
    Task<int> AddAsync(T entity, CancellationToken cancellationToken);
    Task<T> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdateAsync(T entity, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}