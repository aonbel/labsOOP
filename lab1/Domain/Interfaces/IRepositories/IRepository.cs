using Domain.Interfaces.IRepositories;

namespace Domain.Interfaces.IRepositories;

public interface IRepository<T> : ICRRepository<T>
{
    Task UpdateAsync(T entity, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}