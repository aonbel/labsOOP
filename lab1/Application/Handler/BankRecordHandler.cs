using Infrastructure.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Application.Handler;

public class BankRecordHandler(
    IRepository<BankRecordDto> bankRecordRepository,
    TransactionHandler transactionHandler) : IBankRecordHandler
{
    public async Task<BankRecord> GetBankRecordInfoByIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        var bankRecordDto = await bankRecordRepository.GetByIdAsync(bankRecordId, cancellationToken);

        return new BankRecord
        {
            Id = bankRecordDto.Id,
            Name = bankRecordDto.Name,
            Amount = bankRecordDto.Amount,
            IsActive = bankRecordDto.IsActive
        };
    }

    public async Task<BankRecord> CreateBankRecordTransactionsByIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        
    }

    public async Task<BankRecord> CreateBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<int> CreateBankRecordAsync(int userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<BankRecord>> GetAllBankRecordsInfoOfUserByIdAsync(int userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<BankRecord>> GetAllBankRecordsTransactionsByIdAsync(int userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<BankRecord>> GetAllBankRecordsByIdAsync(int userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<BankRecord>> GetAllBankRecordsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteBankRecordByIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateBankRecordInfoByIdAsync(int bankRecordId, BankRecord bankRecord, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}