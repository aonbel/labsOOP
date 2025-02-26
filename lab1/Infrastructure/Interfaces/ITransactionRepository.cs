using Infrastructure.Dtos;

namespace Infrastructure.Interfaces;

public interface ITransactionRepository : IRepository<TransactionDto>
{
    Task<ICollection<TransactionDto>> GetTransactionsAsyncByRecipientBankRecordId(int bankRecordId, CancellationToken cancellationToken);
}