using System.Diagnostics.CodeAnalysis;
using Application.Interfaces;
using Domain.Entities.BankClients;
using Infrastructure.Interfaces;

namespace Application.Handler;

public class ClientHandler<TClient>(IRepository<TClient> clientRepository) : IClientHandler<TClient> where TClient : BankClient
{
    public async Task<int> CreateClientByIdAsync(TClient client, CancellationToken cancellationToken)
    {
        return await clientRepository.AddAsync(client, cancellationToken);
    }

    public async Task UpdateClientByIdAsync(TClient client, CancellationToken cancellationToken)
    {
        await clientRepository.UpdateAsync(client, cancellationToken);
    }

    public async Task<TClient> GetClientByIdAsync(int clientId, CancellationToken cancellationToken)
    {
        return await clientRepository.GetByIdAsync(clientId, cancellationToken);
    }
}