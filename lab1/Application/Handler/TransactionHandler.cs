using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Application.Handler;

public class TransactionHandler(
    IRepository<Transaction> transactionRepository,
    IRepository<BankRecord> bankRecordsRepository) : ITransactionHandler
{
    public async Task<int> RunTransaction(
        int receiverBankRecordId,
        int recipientBankRecordId,
        decimal amount,
        CancellationToken cancellationToken)
    {
        var recipientBankRecord = await bankRecordsRepository.GetByIdAsync(recipientBankRecordId, cancellationToken);
        var receiverBankRecord = await bankRecordsRepository.GetByIdAsync(receiverBankRecordId, cancellationToken);

        var transaction = new Transaction(recipientBankRecord, receiverBankRecord, amount, DateTime.Now);

        recipientBankRecord.Transactions.Add(transaction);

        if (recipientBankRecord.Amount < amount)
        {
            transaction.IsCancelled = true;
            return transaction.Id = await transactionRepository.AddAsync(transaction, cancellationToken);
        }

        recipientBankRecord.Amount -= transaction.Amount;
        receiverBankRecord.Amount += transaction.Amount;

        receiverBankRecord.Transactions.Add(transaction);

        await bankRecordsRepository.UpdateAsync(recipientBankRecord, cancellationToken);
        await bankRecordsRepository.UpdateAsync(receiverBankRecord, cancellationToken);

        return await transactionRepository.AddAsync(transaction, cancellationToken);
    }

    public async Task<ICollection<Transaction>> GetTransactions(int bankRecordId, CancellationToken cancellationToken)
    {
        return (await bankRecordsRepository.GetByIdAsync(bankRecordId, cancellationToken)).Transactions;
    }
}