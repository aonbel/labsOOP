using Infrastructure.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.BankClients;
using Infrastructure.Interfaces;

namespace Application.Handler;

public class BankRecordHandler(
    IBankRecordRepository bankRecordRepository,
    ITransactionHandler transactionHandler) : IBankRecordHandler
{
    public async Task<int> CreateBankRecordAsync(BankClient bankClient, Bank bank, CancellationToken cancellationToken)
    {
        var bankRecordDto = new BankRecordDto
        {
            BankId = bank.Id
        };

        switch (bankClient)
        {
            case CompanyEmployee:
                bankRecordDto.CompanyEmployeeId = bankClient.Id;
                break;
            case Client:
                bankRecordDto.ClientId = bankClient.Id;
                break;
            case Company:
                bankRecordDto.CompanyId = bankClient.Id;
                break;
        }

        return await bankRecordRepository.AddAsync(bankRecordDto, cancellationToken);
    }

    public async Task<BankRecord> GetBankRecordInfoByIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        var bankRecordDto = await bankRecordRepository.GetByIdAsync(bankRecordId, cancellationToken);

        return new BankRecord
        {
            Id = bankRecordDto.Id,
            Name = bankRecordDto.Name,
            Amount = bankRecordDto.Amount,
            IsActive = bankRecordDto.IsActive
        };
    }

    public async Task<BankRecord> GetBankRecordTransactionsByIdAsync(int bankRecordId,
        CancellationToken cancellationToken)
    {
        var bankRecordTransactions =
            await transactionHandler.GetTransactionsInfoAsyncByBankRecordId(bankRecordId, cancellationToken);

        return new BankRecord
        {
            Transactions = bankRecordTransactions
        };
    }

    public async Task<BankRecord> GetBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        var bankRecordDto = await bankRecordRepository.GetByIdAsync(bankRecordId, cancellationToken);
        var bankRecordTransactions =
            await transactionHandler.GetTransactionsInfoAsyncByBankRecordId(bankRecordId, cancellationToken);

        return new BankRecord
        {
            Id = bankRecordDto.Id,
            Name = bankRecordDto.Name,
            Amount = bankRecordDto.Amount,
            IsActive = bankRecordDto.IsActive,
            Transactions = bankRecordTransactions
        };
    }

    public async Task<ICollection<BankRecord>> GetBankRecordsInfoByBankClientIdAsync(BankClient bankClient,
        CancellationToken cancellationToken)
    {
        return (bankClient switch
        {
            CompanyEmployee companyEmployee => await bankRecordRepository
                .GetAllBankRecordsByClientIdAsync(companyEmployee.Id, cancellationToken),
            Company company => await bankRecordRepository
                .GetAllBankRecordsByCompanyIdAsync(company.Id, cancellationToken),
            Client client => await bankRecordRepository
                .GetAllBankRecordsByClientIdAsync(client.Id, cancellationToken),
            _ => throw new ArgumentException(null, nameof(bankClient))
        }).Select(bankRecordDto => new BankRecord
        {
            Id = bankRecordDto.Id,
            Name = bankRecordDto.Name,
            Amount = bankRecordDto.Amount,
            IsActive = bankRecordDto.IsActive
        }).ToList();
    }

    public async Task<ICollection<BankRecord>> GetAllBankRecordsAsync(CancellationToken cancellationToken)
    {
        var bankRecordDtos = await bankRecordRepository.GetAllAsync(cancellationToken);

        var bankRecords = new List<BankRecord>();

        foreach (var bankRecordDto in bankRecordDtos)
        {
            bankRecords.Add(new BankRecord
            {
                Id = bankRecordDto.Id,
                Name = bankRecordDto.Name,
                Amount = bankRecordDto.Amount,
                IsActive = bankRecordDto.IsActive,
                Transactions =
                    await transactionHandler.GetTransactionsInfoAsyncByBankRecordId(bankRecordDto.Id, cancellationToken)
            });
        }

        return bankRecords;
    }

    public async Task DeleteBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        await bankRecordRepository.DeleteAsync(bankRecordId, cancellationToken);
    }

    public async Task UpdateBankRecordInfoByIdAsync(BankRecord bankRecord, CancellationToken cancellationToken)
    {
        var bankRecordDto = new BankRecordDto
        {
            Id = bankRecord.Id,
            Name = bankRecord.Name,
            Amount = bankRecord.Amount,
            IsActive = bankRecord.IsActive
        };

        await bankRecordRepository.UpdateAsync(bankRecordDto, cancellationToken);
    }

    public async Task DeactivateBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        await bankRecordRepository.UpdateStatusOfBankRecordByIdAsync(bankRecordId, false, cancellationToken);
    }

    public async Task ActivateBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        await bankRecordRepository.UpdateStatusOfBankRecordByIdAsync(bankRecordId, true, cancellationToken);
    }

    public async Task WithdrawAmountFromBankRecordByIdAsync(int bankRecordId, decimal withdrawAmount,
        CancellationToken cancellationToken)
    {
        var bankRecordDto = await bankRecordRepository.GetByIdAsync(bankRecordId, cancellationToken);

        if (bankRecordDto.Amount < withdrawAmount)
        {
            throw new ArgumentException("The bank record does not have enough funds");
        }
        
        await bankRecordRepository.UpdateAmountOfBankRecordByIdAsync(bankRecordId, bankRecordDto.Amount - withdrawAmount, cancellationToken);
    }

    public async Task DepositAmountFromBankRecordByIdAsync(int bankRecordId, decimal depositAmount, CancellationToken cancellationToken)
    {
        var bankRecordDto = await bankRecordRepository.GetByIdAsync(bankRecordId, cancellationToken);
        
        await bankRecordRepository.UpdateAmountOfBankRecordByIdAsync(bankRecordId, bankRecordDto.Amount + depositAmount, cancellationToken);
    }
}