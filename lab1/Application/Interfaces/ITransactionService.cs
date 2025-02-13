using Domain.Entities;

namespace Application.Interfaces;

public interface ITransactionService
{
    Task<Transaction> RunTransaction(int receiverBankRecordId, int recipientBankRecordId, decimal amount, CancellationToken cancellationToken);

    Task<ICollection<Transaction>> GetTransactions(int bankRecordId, CancellationToken cancellationToken);
}