using Domain.Dtos;

namespace Domain.Interfaces.IRepositories;

public interface ITransactionRepository : IRepository<TransactionDto>
{
    Task<ICollection<TransactionDto>> GetTransactionsAsyncByRecipientBankRecordId(int bankRecordId, CancellationToken cancellationToken);
}