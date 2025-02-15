using Domain.Entities;

namespace Application.Interfaces;

public interface IBankRecordHandler
{
    Task<BankRecord> GetBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task<int> CreateBankRecord(BankRecord bankRecord, CancellationToken cancellationToken);
}