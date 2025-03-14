using Domain.Entities;

namespace Application.Interfaces;

public interface IBankService
{
    Task<int> CreateBank(Bank bank, CancellationToken cancellationToken);
    Task<Bank> GetBankInfoByIdAsync(int bankId, CancellationToken cancellationToken);
    Task<Bank> GetBankByIdAsync(int bankId, CancellationToken cancellationToken);
    Task<ICollection<Bank>> GetBanksInfoAsync(CancellationToken cancellationToken);
    Task UpdateBank(Bank bank, CancellationToken cancellationToken);
}