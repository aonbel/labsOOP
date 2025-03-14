using Domain.Entities;
using Domain.Entities.BankClients;

namespace Application.Interfaces;

public interface IBankRecordService
{
    Task<int> CreateBankRecordAsync(BankClient bankClient, int bankId, CancellationToken cancellationToken);
    
    Task<BankRecord> GetBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task<ICollection<Transaction>> GetBankRecordTransactionsByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task<ICollection<BankRecord>> GetBankRecordsInfoByBankClientIdAsync(BankClient bankClient, CancellationToken cancellationToken);
    
    Task<ICollection<BankRecord>> GetAllBankRecordsAsync(CancellationToken cancellationToken);
    
    Task DeleteBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task UpdateBankRecordInfoByIdAsync(BankRecord bankRecord, CancellationToken cancellationToken);
    
    Task DeactivateBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task ActivateBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task WithdrawAmountFromBankRecordByIdAsync(int bankRecordId, decimal withdrawAmount, CancellationToken cancellationToken);
    
    Task DepositAmountFromBankRecordByIdAsync(int bankRecordId, decimal depositAmount, CancellationToken cancellationToken);
}