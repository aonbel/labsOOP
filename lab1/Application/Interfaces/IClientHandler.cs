using Domain.Entities.BankClients;

namespace Application.Interfaces;

public interface IClientHandler<TClient> where TClient : BankClient
{
    Task<int> CreateClientByIdAsync(TClient client, CancellationToken cancellationToken);
    Task UpdateClientByIdAsync(TClient client, CancellationToken cancellationToken);
    Task<TClient> GetClientByIdAsync(int clientId, CancellationToken cancellationToken);
}