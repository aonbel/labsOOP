using Domain.Dtos.BankClientDtos;

namespace Domain.Interfaces.IRepositories.IBankClientRepositories;

public interface IClientRepository : IRepository<ClientDto>
{
    Task<ICollection<ClientDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken);
}