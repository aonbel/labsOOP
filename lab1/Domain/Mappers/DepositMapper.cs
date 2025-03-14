using Domain.Dtos.BankServiceDtos;
using Domain.Entities;
using Domain.Entities.BankServices;
using Domain.Interfaces;

namespace Domain.Mappers;

public class DepositMapper : IMapper<Deposit, DepositDto>
{
    public DepositDto Map(Deposit entity)
    {
        return new DepositDto
        {
            Id = entity.Id,
            BankRecordId = entity.Record?.Id ?? 0,
            IsInteractable = entity.IsInteractable,
            InterestRate = entity.InterestRate,
            ClosedAt = entity.ClosedAt,
            CreatedAt = entity.CreatedAt,
            LastUpdatedAt = entity.LastUpdatedAt,
            IsApproved = entity.IsApproved,
            TermInMonths = entity.TermInMonths
        };
    }

    public Deposit Map(DepositDto dto)
    {
        return new Deposit
        {
            Id = dto.Id,
            Record = new BankRecord
            {
                Id = dto.BankRecordId,
            },
            IsInteractable = dto.IsInteractable,
            InterestRate = dto.InterestRate,
            ClosedAt = dto.ClosedAt,
            CreatedAt = dto.CreatedAt,
            LastUpdatedAt = dto.LastUpdatedAt,
            IsApproved = dto.IsApproved,
            TermInMonths = dto.TermInMonths
        };
    }
}