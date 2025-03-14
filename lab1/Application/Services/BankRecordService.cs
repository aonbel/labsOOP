using Application.Interfaces;
using Domain.Dtos;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Interfaces;
using Domain.Interfaces.IRepositories;

namespace Application.Services;

public class BankRecordService(
    IBankRecordRepository bankRecordRepository,
    ITransactionService transactionService,
    IMapper<BankRecord, BankRecordDto> bankRecordMapper) : IBankRecordService
{
    public async Task<int> CreateBankRecordAsync(BankClient bankClient, int bankId, CancellationToken cancellationToken)
    {
        var bankRecordDto = new BankRecordDto
        {
            BankId = bankId
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

    public async Task<BankRecord> GetBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        return bankRecordMapper.Map(await bankRecordRepository.GetByIdAsync(bankRecordId, cancellationToken));
    }

    public async Task<ICollection<Transaction>> GetBankRecordTransactionsByIdAsync(int bankRecordId,
        CancellationToken cancellationToken)
    {
        return await transactionService.GetTransactionsInfoAsyncByBankRecordId(bankRecordId, cancellationToken);
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
            })
            .Select(bankRecordMapper.Map)
            .ToList();
    }

    public async Task<ICollection<BankRecord>> GetAllBankRecordsAsync(CancellationToken cancellationToken)
    {
        var bankRecordDtos = await bankRecordRepository.GetAllAsync(cancellationToken);

        return bankRecordDtos.Select(bankRecordMapper.Map).ToList();
    }

    public async Task DeleteBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        await bankRecordRepository.DeleteAsync(bankRecordId, cancellationToken);
    }

    public async Task UpdateBankRecordInfoByIdAsync(BankRecord bankRecord, CancellationToken cancellationToken)
    {
        await bankRecordRepository.UpdateAsync(bankRecordMapper.Map(bankRecord), cancellationToken);
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

        await bankRecordRepository.UpdateAmountOfBankRecordByIdAsync(bankRecordId,
            bankRecordDto.Amount - withdrawAmount, cancellationToken);
    }

    public async Task DepositAmountFromBankRecordByIdAsync(int bankRecordId, decimal depositAmount,
        CancellationToken cancellationToken)
    {
        var bankRecordDto = await bankRecordRepository.GetByIdAsync(bankRecordId, cancellationToken);

        await bankRecordRepository.UpdateAmountOfBankRecordByIdAsync(bankRecordId, bankRecordDto.Amount + depositAmount,
            cancellationToken);
    }
}