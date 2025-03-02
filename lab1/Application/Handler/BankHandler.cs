using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;

namespace Application.Handler;

public class BankHandler(
    IRepository<BankDto> bankRepository) : IBankHandler
{
    public async Task<int> CreateBank(Bank bank, CancellationToken cancellationToken)
    {
        return await bankRepository.AddAsync(new BankDto
        {
            CompanyType = bank.CompanyType,
            TaxIdentificationNumber = bank.TaxIdentificationNumber,
            TaxIdentificationType = bank.TaxIdentificationType,
            Address = bank.Address,
            BankIdentificationCode = bank.BankIdentificationCode
        }, cancellationToken);
    }

    public async Task<Bank> GetBankInfoByIdAsync(int bankId, CancellationToken cancellationToken)
    {
        var bankDto = await bankRepository.GetByIdAsync(bankId, cancellationToken);

        return new Bank
        {
            Id = bankDto.Id,
            CompanyType = bankDto.CompanyType,
            TaxIdentificationNumber = bankDto.TaxIdentificationNumber,
            TaxIdentificationType = bankDto.TaxIdentificationType,
            Address = bankDto.Address,
            BankIdentificationCode = bankDto.BankIdentificationCode
        };
    }

    public async Task<Bank> GetBankByIdAsync(int bankId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<Bank>> GetBanksInfoAsync(CancellationToken cancellationToken)
    {
        return (await bankRepository.GetAllAsync(cancellationToken)).Select(bankDto => new Bank
        {
            Id = bankDto.Id,
            CompanyType = bankDto.CompanyType,
            TaxIdentificationNumber = bankDto.TaxIdentificationNumber,
            TaxIdentificationType = bankDto.TaxIdentificationType,
            Address = bankDto.Address,
            BankIdentificationCode = bankDto.BankIdentificationCode
        }).ToList();
    }

    public async Task UpdateBank(Bank bank, CancellationToken cancellationToken)
    {
        await bankRepository.UpdateAsync(new BankDto
        {
            Id = bank.Id,
            CompanyType = bank.CompanyType,
            TaxIdentificationNumber = bank.TaxIdentificationNumber,
            TaxIdentificationType = bank.TaxIdentificationType,
            Address = bank.Address,
            BankIdentificationCode = bank.BankIdentificationCode
        }, cancellationToken);
    }
}