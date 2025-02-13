using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> GetTransactionByIdAsync(int transactionId, CancellationToken cancellationToken = default);
    Task UpdateTransactionByIdAsync(int transactionId, Transaction transaction, CancellationToken cancellationToken = default);
    Task<int> AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default);
}