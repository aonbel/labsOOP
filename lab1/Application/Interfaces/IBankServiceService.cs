using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;

namespace Application.Interfaces;

public interface IBankServiceService
{
     Task<int> CreateBankService(int bankId, BankClient bankClient, BankService bankService, CancellationToken cancellationToken);
     Task<int> CreateBankService(BankService bankService, CancellationToken cancellationToken);
     Task<BankService> GetBankServiceByIdAsync(BankService bankService, CancellationToken cancellationToken);
     Task<ICollection<BankService>> GetBankServicesInfoByBankClientIdAsync(BankClient bankClient, CancellationToken cancellationToken);
     Task UpdateBankService(BankService service, CancellationToken cancellationToken);
     Task DeleteBankService(BankService service, CancellationToken cancellationToken);
}

