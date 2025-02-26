using Domain.Entities;
using Infrastructure.Dtos;

namespace Application.Interfaces;

public interface IBankRecordHandler
{
    Task<int> CreateBankRecordAsync(int userId, CancellationToken cancellationToken);
    
    Task<BankRecord> GetBankRecordInfoByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task<BankRecord> GetBankRecordTransactionsByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task<BankRecord> GetBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task<ICollection<BankRecord>> GetAllBankRecordsInfoOfUserByIdAsync(int userId, CancellationToken cancellationToken);
    
    Task<ICollection<BankRecord>> GetAllBankRecordsTransactionsByIdAsync(int userId, CancellationToken cancellationToken);
    
    Task<ICollection<BankRecord>> GetAllBankRecordsByIdAsync(int userId, CancellationToken cancellationToken);
    
    Task<ICollection<BankRecord>> GetAllBankRecordsAsync(CancellationToken cancellationToken);
    
    Task DeleteBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task UpdateBankRecordInfoByIdAsync(int bankRecordId, BankRecord bankRecord, CancellationToken cancellationToken);
}