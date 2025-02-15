using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Application.Handler;

public class BankHandler(IRepository<Bank> bankRepository) : IBankHandler
{
    public async Task<int> CreateBank(Bank bank, CancellationToken cancellationToken)
    {
        return await bankRepository.AddAsync(bank, cancellationToken);
    }

    public async Task<Bank> GetBankByIdAsync(int bankId, CancellationToken cancellationToken)
    {
        return await bankRepository.GetByIdAsync(bankId, cancellationToken);
    }

    public async Task<ICollection<Bank>> GetBanksAsync(CancellationToken cancellationToken)
    {
        return await bankRepository.GetAllAsync(cancellationToken);
    }

    public async Task UpdateBank(Bank bank, CancellationToken cancellationToken)
    {
        await bankRepository.UpdateAsync(bank, cancellationToken);
    }
}