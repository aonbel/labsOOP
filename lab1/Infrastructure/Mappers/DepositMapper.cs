using Domain.Entities.BankServices;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;

namespace Infrastructure.Mappers;

public class DepositMapper : IMapper<Deposit, DepositDto>
{
    public DepositDto Map(Deposit entity)
    {
        return new DepositDto
        {
            Id = entity.Id,
            Name = entity.Name,
            BankRecordId = entity.Record.Id,
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
            Name = dto.Name,
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