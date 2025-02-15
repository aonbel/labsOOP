using Domain.Entities;

namespace Application.Interfaces;

public interface IBankHandler
{
    Task<int> CreateBank(Bank bank, CancellationToken cancellationToken);
    Task<Bank> GetBankByIdAsync(int bankId, CancellationToken cancellationToken);
    Task<ICollection<Bank>> GetBanksAsync(CancellationToken cancellationToken);
    Task UpdateBank(Bank bank, CancellationToken cancellationToken);
}