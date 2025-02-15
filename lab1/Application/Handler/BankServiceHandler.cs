using System.Net.Sockets;
using Application.Interfaces;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Infrastructure.Interfaces;

namespace Application.Handler;

public class BankServiceHandler<TService, TClient>(
    IRepository<TService> bankServiceRepository,
    IRepository<TClient> bankClientRepository)
    : IBankServiceHandler<TService, TClient> where TService : BankService, new() where TClient : BankClient
{
    public async Task<int> CreateBankService(int bankClientId, TService bankService,
        CancellationToken cancellationToken)
    {
        var bankClient = await bankClientRepository.GetByIdAsync(bankClientId, cancellationToken);

        bankClient.Services.Add(bankService);

        return bankService.Id = await bankServiceRepository.AddAsync(bankService, cancellationToken);
    }

    public async Task<TService> GetBankServiceByIdAsync(int bankServiceId, CancellationToken cancellationToken)
    {
        return await bankServiceRepository.GetByIdAsync(bankServiceId, cancellationToken);
    }

    public async Task<ICollection<TService>> GetBankServicesByBankClientIdAsync(int bankClientId,
        CancellationToken cancellationToken)
    {
        var services = (await bankClientRepository.GetByIdAsync(bankClientId, cancellationToken)).Services;
        
        var result = new List<TService>();
        
        foreach (var service in services)
        {
            if (service is TService tService)
            {
                result.Add(tService);
            }
        }
        
        return result;
    }

    public async Task UpdateBankService(TService service, CancellationToken cancellationToken)
    {
        await bankServiceRepository.UpdateAsync(service, cancellationToken);
    }
}