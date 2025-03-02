using Infrastructure.Dtos;

namespace Infrastructure.Interfaces;

public interface IDepositRepository : IRepository<DepositDto>
{
    public Task<ICollection<DepositDto>> GetByBankRecordIdAsync(int bankRecordId, CancellationToken cancellationToken);
}