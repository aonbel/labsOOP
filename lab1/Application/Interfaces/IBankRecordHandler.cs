using Domain.Entities;
using Domain.Entities.BankClients;
using Infrastructure.Dtos;

namespace Application.Interfaces;

public interface IBankRecordHandler
{
    Task<int> CreateBankRecordAsync(BankClient bankClient, Bank bank, CancellationToken cancellationToken);
    
    Task<BankRecord> GetBankRecordInfoByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task<BankRecord> GetBankRecordTransactionsByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task<BankRecord> GetBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task<ICollection<BankRecord>> GetBankRecordsInfoByBankClientIdAsync(BankClient bankClient, CancellationToken cancellationToken);
    
    Task<ICollection<BankRecord>> GetAllBankRecordsAsync(CancellationToken cancellationToken);
    
    Task DeleteBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task UpdateBankRecordInfoByIdAsync(BankRecord bankRecord, CancellationToken cancellationToken);
    
    Task DeactivateBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task ActivateBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken);
    
    Task WithdrawAmountFromBankRecordByIdAsync(int bankRecordId, decimal withdrawAmount, CancellationToken cancellationToken);
    
    Task DepositAmountFromBankRecordByIdAsync(int bankRecordId, decimal depositAmount, CancellationToken cancellationToken);
}