using Application.Interfaces;
using Domain.Entities;

namespace Application.Reverters;

public class TransactionReverter(ITransactionService transactionService) : IReverter<Transaction>
{
    public async Task RevertCreateActionAsync(int entityId, CancellationToken cancellationToken)
    {
        var currTransaction = await transactionService.GetTransactionByIdAsync(entityId, cancellationToken);
        
        await transactionService.RunTransactionAsync(currTransaction.ReceiverBankRecord.Id, currTransaction.RecipientBankRecord.Id, currTransaction.Amount, cancellationToken);
    }

    public async Task RevertUpdateActionAsync(Transaction state, CancellationToken cancellationToken)
    {
        var currTransaction = await transactionService.GetTransactionByIdAsync(state.Id, cancellationToken);
        
        currTransaction.Amount = state.Amount - currTransaction.Amount;

        if (currTransaction.Amount < 0)
        {
            currTransaction.Amount = -currTransaction.Amount;
            
            (currTransaction.ReceiverBankRecord, currTransaction.RecipientBankRecord) = (currTransaction.RecipientBankRecord, currTransaction.ReceiverBankRecord);
        }
        
        await transactionService.RunTransactionAsync(currTransaction.ReceiverBankRecord.Id, currTransaction.RecipientBankRecord.Id, currTransaction.Amount, cancellationToken);
    }

    public async Task RevertDeleteActionAsync(Transaction state, CancellationToken cancellationToken)
    {
        (state.ReceiverBankRecord, state.RecipientBankRecord) = (state.RecipientBankRecord, state.ReceiverBankRecord);
        
        await transactionService.RunTransactionAsync(state.RecipientBankRecord.Id, state.ReceiverBankRecord.Id, state.Amount, cancellationToken);
    }
}