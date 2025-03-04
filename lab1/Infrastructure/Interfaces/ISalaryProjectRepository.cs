using Infrastructure.Dtos;

namespace Infrastructure.Interfaces;

public interface ISalaryProjectRepository : IRepository<SalaryProjectDto>
{
    public Task<ICollection<SalaryProjectDto>> GetByBankRecordIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    public Task<ICollection<SalaryProjectDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken);
}