using Domain.Entities.BankClients;

namespace Application.Interfaces;

public interface IBankClientHandler
{
    Task<int> CreateClientAsync(BankClient bankClient, CancellationToken cancellationToken);
    Task UpdateClientByIdAsync(BankClient bankClient, CancellationToken cancellationToken);
    Task<BankClient> GetClientInfoByIdAsync(BankClient bankClient, CancellationToken cancellationToken);
    Task<BankClient> GetClientServicesAndRecordsInfoByIdAsync(BankClient bankClient, CancellationToken cancellationToken);
    Task<BankClient> GetClientByIdAsync(BankClient bankClient, CancellationToken cancellationToken);
    Task<ICollection<CompanyEmployee>> GetCompanyEmployeesInfoBySalaryProjectIdAsync(int salaryProjectId, CancellationToken cancellationToken);
}