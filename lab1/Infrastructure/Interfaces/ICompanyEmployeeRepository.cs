using Domain.Entities.BankClients;
using Infrastructure.Dtos;

namespace Infrastructure.Interfaces;

public interface ICompanyEmployeeRepository : IRepository<CompanyEmployeeDto>
{
    public Task<ICollection<CompanyEmployeeDto>> GetCompanyEmployeesBySalaryProjectIdAsync(int salaryProjectId, CancellationToken cancellationToken);
    Task<ICollection<CompanyEmployeeDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken);
}