using System.Transactions;
using Application.Interfaces;
using Domain.Entities.BankClients;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Transaction = Domain.Entities.Transaction;

namespace Application.Handler;

public class TransactionHandler(
    ITransactionRepository transactionRepository,
    IBankRecordRepository bankRecordsRepository,
    IRepository<ClientDto> clientRepository,
    IRepository<CompanyDto> companyRepository,
    ICompanyEmployeeRepository companyEmployeeRepository) : ITransactionHandler
{
    public async Task<int> RunTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        var transactionDto = new TransactionDto
        {
            Name = transaction.Name,
            Amount = transaction.Amount,
            Date = transaction.Date,
            RecipientBankRecordId = transaction.ReceiverBankRecord.Id,
            ReceiverBankRecordId = transaction.ReceiverBankRecord.Id
        };

        var recipientBankRecord =
            await bankRecordsRepository.GetByIdAsync(transaction.RecipientBankRecord.Id, cancellationToken);

        transactionDto.IsCancelled = recipientBankRecord.Amount < transaction.Amount;

        if (transaction.IsCancelled)
        {
            return await transactionRepository.AddAsync(transactionDto, cancellationToken);
        }

        var receiverBankRecord =
            await bankRecordsRepository.GetByIdAsync(transaction.ReceiverBankRecord.Id, cancellationToken);

        recipientBankRecord.Amount -= transaction.Amount;
        receiverBankRecord.Amount += transaction.Amount;

        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            await bankRecordsRepository.UpdateAsync(receiverBankRecord, cancellationToken);
            await bankRecordsRepository.UpdateAsync(recipientBankRecord, cancellationToken);

            var transactionId = await transactionRepository.AddAsync(transactionDto, cancellationToken);

            transactionScope.Complete();

            return transactionId;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ICollection<Transaction>> GetTransactionsInfoAsyncByBankRecordId(int bankRecordId,
        CancellationToken cancellationToken)
    {
        var transactionDtos =
            await transactionRepository.GetTransactionsAsyncByRecipientBankRecordId(bankRecordId, cancellationToken);

        var transactions = transactionDtos.Select(transactionDto => new Transaction
        {
            Id = transactionDto.Id, 
            Name = transactionDto.Name, 
            Amount = transactionDto.Amount,
            Date = transactionDto.Date
        }).ToList();

        return transactions;
    }

    public async Task<ICollection<Transaction>> GetTransactionsInfoAsyncByBankClientId(BankClient bankClient,
        CancellationToken cancellationToken)
    {
        var bankClientRecordIds = bankClient switch
        {
            CompanyEmployee companyEmployee => (await companyEmployeeRepository.GetByIdAsync(companyEmployee.Id,
                cancellationToken)).RecordIds,
            Client client => (await clientRepository.GetByIdAsync(client.Id, cancellationToken)).RecordIds,
            Company company => (await companyRepository.GetByIdAsync(company.Id, cancellationToken)).RecordIds,
            _ => throw new ArgumentException(null, nameof(bankClient))
        };

        var transactions = new List<Transaction>();

        foreach (var bankRecordId in bankClientRecordIds)
        {
            transactions.AddRange(await GetTransactionsInfoAsyncByBankRecordId(bankRecordId, cancellationToken));
        }

        return transactions;
    }
}