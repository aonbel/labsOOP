using Domain.Entities.Users;
using Infrastructure.Dtos;

namespace Infrastructure.Interfaces;

public interface IUserRepository : IRepository<UserDto>
{
    public Task<UserDto> GetByLoginAsync(string login, CancellationToken cancellationToken);
}