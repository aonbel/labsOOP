using Infrastructure.Dtos;

namespace Infrastructure.Interfaces;

public interface IInstallmentRepository : IRepository<InstallmentDto>
{
    public Task<ICollection<InstallmentDto>> GetByBankRecordIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    public Task<ICollection<InstallmentDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken);
}