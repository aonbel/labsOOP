using Infrastructure.Dtos;

namespace Infrastructure.Interfaces;

public interface ICompanyRepository : IRepository<CompanyDto>
{ 
    Task<ICollection<CompanyDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken);
}