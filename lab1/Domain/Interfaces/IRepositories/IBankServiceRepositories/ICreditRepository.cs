using Domain.Dtos.BankServiceDtos;

namespace Domain.Interfaces.IRepositories.IBankServiceRepositories;

public interface ICreditRepository : IRepository<CreditDto>
{
    public Task<ICollection<CreditDto>> GetByBankRecordIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    public Task<ICollection<CreditDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken);
}