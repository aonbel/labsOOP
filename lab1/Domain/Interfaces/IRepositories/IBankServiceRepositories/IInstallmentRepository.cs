using Domain.Dtos.BankServiceDtos;

namespace Domain.Interfaces.IRepositories.IBankServiceRepositories;

public interface IInstallmentRepository : IRepository<InstallmentDto>
{
    public Task<ICollection<InstallmentDto>> GetByBankRecordIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    public Task<ICollection<InstallmentDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken);
}