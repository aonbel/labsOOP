using Domain.Dtos;
using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Mappers;

public class TransactionMapper : IMapper<Transaction, TransactionDto>
{
    public TransactionDto Map(Transaction entity)
    {
        return new TransactionDto
        {
            Amount = entity.Amount,
            Date = entity.Date,
            Id = entity.Id,
            ReceiverBankRecordId = entity.ReceiverBankRecord?.Id ?? 0,
            RecipientBankRecordId = entity.RecipientBankRecord?.Id ?? 0,
        };
    }

    public Transaction Map(TransactionDto dto)
    {
        return new Transaction
        {
            Amount = dto.Amount,
            Date = dto.Date,
            Id = dto.Id,
            ReceiverBankRecord = new BankRecord
            {
                Id = dto.ReceiverBankRecordId
            },
            RecipientBankRecord = new BankRecord
            {
                Id = dto.RecipientBankRecordId
            }
        };
    }
}