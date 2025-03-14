using Domain.Dtos.BankClientDtos;

namespace Domain.Interfaces.IRepositories.IBankClientRepositories;

public interface ICompanyRepository : IRepository<CompanyDto>
{ 
    Task<ICollection<CompanyDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken);
}