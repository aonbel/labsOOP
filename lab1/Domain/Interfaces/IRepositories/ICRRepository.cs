namespace Domain.Interfaces.IRepositories;

public interface ICRRepository<T>
{
    Task<int> AddAsync(T entity, CancellationToken cancellationToken);
    Task<T> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken);
}