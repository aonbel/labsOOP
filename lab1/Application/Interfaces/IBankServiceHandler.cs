using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;

namespace Application.Interfaces;

public interface IBankServiceHandler
{
     Task<int> CreateBankService(BankClient bankClient, BankService bankService, Bank bank, CancellationToken cancellationToken);
     Task<BankService> GetBankServiceByIdAsync(BankService bankService, CancellationToken cancellationToken);
     Task<ICollection<BankService>> GetBankServicesInfoByBankClientIdAsync(BankClient bankClient, CancellationToken cancellationToken);
     Task UpdateBankService(BankService service, CancellationToken cancellationToken);
}

