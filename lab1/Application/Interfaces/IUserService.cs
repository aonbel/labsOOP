using Domain.Entities;

namespace Application.Interfaces;

public interface IUserService
{
    public Task<int> CreateUserAsync(User user, CancellationToken cancellationToken);
    
    public Task<User> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    
    public Task<User> GetUserByLoginAsync(string login, CancellationToken cancellationToken);
    
    public Task UpdateAsync(User user, CancellationToken cancellationToken);
}