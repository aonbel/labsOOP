using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IBankRecordsRepository
{
    Task<BankRecord> GetBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken = default);
    Task UpdateBankRecordByIdAsync(int bankRecordId, BankRecord bankRecord, CancellationToken cancellationToken = default);
    Task AddBankRecordAsync(BankRecord bankRecord, CancellationToken cancellationToken = default);
}