using Domain.Entities;
using Infrastructure.Dtos;

namespace Infrastructure.Interfaces;

public interface IBankRecordRepository : IRepository<BankRecordDto>
{
    public Task<ICollection<BankRecordDto>> GetAllBankRecordsByClientIdAsync(int clientId, CancellationToken cancellationToken);
    
    public Task<ICollection<BankRecordDto>> GetAllBankRecordsByCompanyIdAsync(int companyId, CancellationToken cancellationToken);
    
    public Task<ICollection<BankRecordDto>> GetAllBankRecordsByCompanyEmployeeIdAsync(int companyEmployeeId, CancellationToken cancellationToken);
    
    public Task UpdateStatusOfBankRecordByIdAsync(int bankRecordId, bool status, CancellationToken cancellationToken);
    
    public Task UpdateAmountOfBankRecordByIdAsync(int bankRecordId, decimal amount, CancellationToken cancellationToken);
}