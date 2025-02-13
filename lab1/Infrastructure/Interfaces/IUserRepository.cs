using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<int> AddUserAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
}