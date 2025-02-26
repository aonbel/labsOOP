using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Infrastructure.Repositories;

namespace Application.Interfaces;

public interface IBankServiceHandler<TService, TClient>
     where TService : BankService
     where TClient : BankClient
{
     Task<int> CreateBankService(int bankClientId, TService bankService, CancellationToken cancellationToken);
     Task<TService> GetBankServiceByIdAsync(int bankServiceId, CancellationToken cancellationToken);
     Task<ICollection<TService>> GetBankServicesByBankClientIdAsync(int bankClientId, CancellationToken cancellationToken);
     Task UpdateBankService(TService service, CancellationToken cancellationToken);
}

