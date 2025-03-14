using Application.Interfaces;
using Domain.Entities.BankServices;

namespace Application.Reverters;

public class BankServiceReverter<TBankService>(IBankServiceService bankServiceService) : IReverter<TBankService> where TBankService : BankService, new()
{
    public async Task RevertCreateActionAsync(int entityId, CancellationToken cancellationToken)
    {
        await bankServiceService.DeleteBankService(new TBankService
        {
            Id = entityId,
        }, cancellationToken);
    }

    public async Task RevertUpdateActionAsync(TBankService entityState, CancellationToken cancellationToken)
    {
        await bankServiceService.UpdateBankService(entityState, cancellationToken);
    }

    public async Task RevertDeleteActionAsync(TBankService entityState, CancellationToken cancellationToken)
    { 
        await bankServiceService.CreateBankService(entityState, cancellationToken);
    }
}