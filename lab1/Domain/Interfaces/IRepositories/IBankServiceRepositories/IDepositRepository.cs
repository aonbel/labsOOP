using Domain.Dtos.BankServiceDtos;

namespace Domain.Interfaces.IRepositories.IBankServiceRepositories;

public interface IDepositRepository : IRepository<DepositDto>
{
    public Task<ICollection<DepositDto>> GetByBankRecordIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    public Task<ICollection<DepositDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken);
}