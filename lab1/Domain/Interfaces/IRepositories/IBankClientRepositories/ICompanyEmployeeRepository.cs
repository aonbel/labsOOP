using Domain.Dtos.BankClientDtos;

namespace Domain.Interfaces.IRepositories.IBankClientRepositories;

public interface ICompanyEmployeeRepository : IRepository<CompanyEmployeeDto>
{
    public Task<ICollection<CompanyEmployeeDto>> GetCompanyEmployeesBySalaryProjectIdAsync(int salaryProjectId, CancellationToken cancellationToken);
    Task<ICollection<CompanyEmployeeDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken);
}