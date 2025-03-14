using Domain.Entities;
using Domain.Entities.BankClients;

namespace Application.Interfaces;

public interface ITransactionService
{
    Task<int> RunTransactionAsync(int recipientBankRecordId, int receiverBankRecordId, decimal amount, CancellationToken cancellationToken);
    
    Task<Transaction> GetTransactionByIdAsync(int id, CancellationToken cancellationToken);

    Task<ICollection<Transaction>> GetTransactionsInfoAsyncByBankRecordId(int bankRecordId, CancellationToken cancellationToken);
    
    Task<ICollection<Transaction>> GetTransactionsInfoAsyncByBankClientId(BankClient bankClient, CancellationToken cancellationToken);
}