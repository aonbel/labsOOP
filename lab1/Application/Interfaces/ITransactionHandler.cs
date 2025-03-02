using Domain.Entities;
using Domain.Entities.BankClients;

namespace Application.Interfaces;

public interface ITransactionHandler
{
    Task<int> RunTransactionAsync(Transaction transaction, CancellationToken cancellationToken);

    Task<ICollection<Transaction>> GetTransactionsInfoAsyncByBankRecordId(int bankRecordId, CancellationToken cancellationToken);
    
    Task<ICollection<Transaction>> GetTransactionsInfoAsyncByBankClientId(BankClient bankClient, CancellationToken cancellationToken);
}