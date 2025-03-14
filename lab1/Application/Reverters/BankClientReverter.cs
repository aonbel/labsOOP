using Application.Interfaces;
using Domain.Entities.BankClients;

namespace Application.Reverters;

public class BankClientReverter<TBankClient>(IBankClientService bankClientService)
    : IReverter<TBankClient> where TBankClient : BankClient, new()
{
    public async Task RevertCreateActionAsync(int entityId, CancellationToken cancellationToken)
    {
        await bankClientService.DeleteClientByIdAsync(new TBankClient
        {
            Id = entityId
        }, cancellationToken);
    }

    public async Task RevertUpdateActionAsync(TBankClient entityState, CancellationToken cancellationToken)
    {
        await bankClientService.UpdateClientByIdAsync(entityState, cancellationToken);
    }

    public async Task RevertDeleteActionAsync(TBankClient entityState, CancellationToken cancellationToken)
    {
        await bankClientService.CreateClientAsync(entityState, cancellationToken);
    }
}