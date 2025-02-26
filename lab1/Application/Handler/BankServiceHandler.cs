using Application.Interfaces;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Infrastructure.Dtos;
using Infrastructure.Repositories;

namespace Application.Handler;

public class BankServiceHandler<TService, TClient>(
    BankRecordRepository bankRecordRepository,
    CreditRepository creditRepository,
    DepositRepository depositRepository,
    InstallmentRepository installmentRepository, 
    ClientHandler<TClient> clientHandler,
    BankRecordHandler bankRecordHandler)
    : IBankServiceHandler<TService, TClient> where TService : BankService, new() where TClient : BankClient
{
    public async Task<int> CreateBankService(int bankClientId, TService bankService,
        CancellationToken cancellationToken)
    {
        var bankRecordDto = new BankRecordDto
        {
            BankClientId = bankClientId
        };
        
        var recordId = await bankRecordHandler.CreateBankRecordAsync(bankClientId, bankRecordDto, cancellationToken);
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