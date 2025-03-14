using Application.Interfaces;
using Domain.Entities;

namespace Application.Reverters;

public class BankRecordReverter(IBankRecordService bankRecordService) : IReverter<BankRecord>
{
    public async Task RevertCreateActionAsync(int entityId, CancellationToken cancellationToken)
    {
        await bankRecordService.DeleteBankRecordByIdAsync(entityId, cancellationToken);
    }

    public async Task RevertUpdateActionAsync(BankRecord entityState, CancellationToken cancellationToken)
    {
        await bankRecordService.UpdateBankRecordInfoByIdAsync(entityState, cancellationToken);
    }

    public async Task RevertDeleteActionAsync(BankRecord entityState, CancellationToken cancellationToken)
    {
        await bankRecordService.CreateBankRecordAsync(entityState.BankClient, entityState.Bank.Id, cancellationToken);
    }
}