using Application.Interfaces;
using Domain.Dtos;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.IRepositories;

namespace Application.Services;

public class BankService(
    IRepository<BankDto> bankRepository,
    IMapper<Bank, BankDto> bankMapper) : IBankService
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