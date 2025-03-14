using Domain.Dtos;

namespace Domain.Interfaces.IRepositories;

public interface IUserActionRepository : ICRRepository<UserActionDto>
{
    Task<ICollection<UserActionDto>> GetAllActionsByUserIdAsync(int userId, CancellationToken cancellationToken);
}