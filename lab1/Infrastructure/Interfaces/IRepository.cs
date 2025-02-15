using Domain.Entities.Core;

namespace Infrastructure.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<int> AddAsync(T entity, CancellationToken cancellationToken);
    Task<T> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdateAsync(T entity, CancellationToken cancellationToken);
}