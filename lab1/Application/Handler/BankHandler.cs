using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;

namespace Application.Handler;

public class BankHandler(
    IRepository<BankDto> bankRepository,
    IMapper<Bank, BankDto> bankMapper) : IBankHandler
{
    public async Task<int> CreateBank(Bank bank, CancellationToken cancellationToken)
    {
        return await bankRepository.AddAsync(bankMapper.Map(bank), cancellationToken);
    }

    public async Task<Bank> GetBankInfoByIdAsync(int bankId, CancellationToken cancellationToken)
    {
        return bankMapper.Map(await bankRepository.GetByIdAsync(bankId, cancellationToken));
    }

    public async Task<Bank> GetBankByIdAsync(int bankId, CancellationToken cancellationToken)
    {
        return bankMapper.Map(await bankRepository.GetByIdAsync(bankId, cancellationToken));
    }

    public async Task<ICollection<Bank>> GetBanksInfoAsync(CancellationToken cancellationToken)
    {
        return (await bankRepository.GetAllAsync(cancellationToken)).Select(bankMapper.Map).ToList();
    }

    public async Task UpdateBank(Bank bank, CancellationToken cancellationToken)
    {
        await bankRepository.UpdateAsync(bankMapper.Map(bank), cancellationToken);
    }
}