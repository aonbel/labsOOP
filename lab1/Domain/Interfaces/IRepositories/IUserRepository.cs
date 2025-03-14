using Domain.Dtos;

namespace Domain.Interfaces.IRepositories;

public interface IUserRepository : IRepository<UserDto>
{
    public Task<UserDto> GetByLoginAsync(string login, CancellationToken cancellationToken);
}