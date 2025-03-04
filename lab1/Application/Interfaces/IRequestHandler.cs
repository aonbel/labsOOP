using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;

namespace Application.Interfaces;

public interface IRequestHandler
{
    public Task<int> CreateBankServiceRequestAsync(Bank bank, BankClient bankClient, BankService bankService, CancellationToken cancellationToken);
    
    public Task<ICollection<BankService>> GetBankServiceRequestsAsync(CancellationToken cancellationToken);
    
    public Task ApproveBankServiceRequestAsync(BankService bankService, CancellationToken cancellationToken);
    
    public Task DisapproveBankServiceRequestAsync(BankService bankService, CancellationToken cancellationToken);
    
    public Task<int> CreateBankClientRequestAsync(BankClient bankClient, CancellationToken cancellationToken);
    
    public Task<ICollection<BankClient>> GetBankClientRequestsAsync(CancellationToken cancellationToken);
    
    public Task ApproveBankClientRequestAsync(BankClient bankClient, CancellationToken cancellationToken);
    
    public Task DisapproveBankClientRequestAsync(BankClient bankClient, CancellationToken cancellationToken);   
}