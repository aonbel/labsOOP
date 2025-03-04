using Domain.Entities.BankClients;
using Infrastructure.Dtos;

namespace Infrastructure.Interfaces;

public interface IClientRepository : IRepository<ClientDto>
{
    Task<ICollection<ClientDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken);
}