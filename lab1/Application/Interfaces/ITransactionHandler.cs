using Domain.Entities;

namespace Application.Interfaces;

public interface ITransactionHandler
{
    Task<int> RunTransactionAsync(Transaction transaction, CancellationToken cancellationToken);

    Task<ICollection<Transaction>> GetTransactionsInfoAsyncByBankRecordId(int bankRecordId, CancellationToken cancellationToken);
    
    Task<ICollection<Transaction>> GetTransactionsInfoAsyncByClientId(int clientId, CancellationToken cancellationToken);
}