using System.Transactions;
using Application.Interfaces;
using Domain.Dtos;
using Domain.Dtos.BankClientDtos;
using Domain.Entities.BankClients;
using Domain.Interfaces;
using Domain.Interfaces.IRepositories;
using Domain.Interfaces.IRepositories.IBankClientRepositories;
using Transaction = Domain.Entities.Transaction;

namespace Application.Services;

public class TransactionService(
    ITransactionRepository transactionRepository,
    IBankRecordRepository bankRecordsRepository,
    IRepository<ClientDto> clientRepository,
    IRepository<CompanyDto> companyRepository,
    ICompanyEmployeeRepository companyEmployeeRepository,
    IMapper<Transaction, TransactionDto> transactionMapper) : ITransactionService
{
    public async Task<int> RunTransactionAsync(int recipientBankRecordId, int receiverBankRecordId, decimal amount, CancellationToken cancellationToken)
    {
        var transactionDto = new TransactionDto
        {
            Amount = amount,
            RecipientBankRecordId = recipientBankRecordId,
            ReceiverBankRecordId = receiverBankRecordId,
            Date = DateTime.Now,
        };

        var recipientBankRecord =
            await bankRecordsRepository.GetByIdAsync(transactionDto.RecipientBankRecordId, cancellationToken);

        transactionDto.IsCancelled = recipientBankRecord.Amount < transactionDto.Amount;

        if (transactionDto.IsCancelled)
        {
            return await transactionRepository.AddAsync(transactionDto, cancellationToken);
        }

        var receiverBankRecord =
            await bankRecordsRepository.GetByIdAsync(transactionDto.ReceiverBankRecordId, cancellationToken);

        recipientBankRecord.Amount -= transactionDto.Amount;
        receiverBankRecord.Amount += transactionDto.Amount;

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

    public async Task<Transaction> GetTransactionByIdAsync(int id, CancellationToken cancellationToken)
    {
        return transactionMapper.Map(await transactionRepository.GetByIdAsync(id, cancellationToken));
    }

    public async Task<ICollection<Transaction>> GetTransactionsInfoAsyncByBankRecordId(int bankRecordId,
        CancellationToken cancellationToken)
    {
        var transactionDtos =
            await transactionRepository.GetTransactionsAsyncByRecipientBankRecordId(bankRecordId, cancellationToken);

        var transactions = transactionDtos.Select(transactionMapper.Map).ToList();

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