using Domain.Entities;
using Infrastructure.Dtos;

namespace Infrastructure.Interfaces;

public interface ICreditRepository : IRepository<CreditDto>
{
    public Task<ICollection<CreditDto>> GetByBankRecordIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    public Task<ICollection<CreditDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken);
}