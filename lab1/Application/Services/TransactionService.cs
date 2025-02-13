using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Application.Services;

public class TransactionService(ITransactionRepository transactionRepository, IBankRecordsRepository bankRecordsRepository) : ITransactionService
{
    public async Task<Transaction> RunTransaction(
        int receiverBankRecordId, 
        int recipientBankRecordId, 
        decimal amount, 
        CancellationToken cancellationToken)
    {
        var recipientBankRecord = await bankRecordsRepository.GetBankRecordByIdAsync(recipientBankRecordId, cancellationToken);
        var receiverBankRecord = await bankRecordsRepository.GetBankRecordByIdAsync(receiverBankRecordId, cancellationToken);
        
        var transaction = new Transaction(recipientBankRecord, receiverBankRecord, amount, DateTime.Now);
        
        recipientBankRecord.Transactions.Add(transaction);

        if (recipientBankRecord.Amount < amount)
        {
            transaction.Status = false;
            await transactionRepository.AddTransactionAsync(transaction, cancellationToken);
            return transaction;
        }

        recipientBankRecord.Amount -= transaction.Amount;
        receiverBankRecord.Amount += transaction.Amount;
        
        receiverBankRecord.Transactions.Add(transaction);

        await bankRecordsRepository.UpdateBankRecordByIdAsync(recipientBankRecordId, recipientBankRecord, cancellationToken);
        await bankRecordsRepository.UpdateBankRecordByIdAsync(receiverBankRecordId, receiverBankRecord, cancellationToken);
        await transactionRepository.AddTransactionAsync(transaction, cancellationToken);
        
        return transaction;
    }

    public async Task<ICollection<Transaction>> GetTransactions(int bankRecordId, CancellationToken cancellationToken)
    {
        var bankRecord = await bankRecordsRepository.GetBankRecordByIdAsync(bankRecordId, cancellationToken);
        
        return bankRecord.Transactions;
    }
}