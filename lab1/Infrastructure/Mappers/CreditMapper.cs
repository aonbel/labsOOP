using Domain.Entities;
using Domain.Entities.BankServices;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;

namespace Infrastructure.Mappers;

public class CreditMapper : IMapper<Credit, CreditDto>
{
    public CreditDto Map(Credit entity)
    {
        return new CreditDto
        {
            Id = entity.Id,
            Name = entity.Name,
            BankRecordId = entity.Record.Id,
            Amount = entity.Amount,
            InterestRate = entity.InterestRate,
            ClosedAt = entity.ClosedAt,
            CreatedAt = entity.CreatedAt,
            LastUpdatedAt = entity.LastUpdatedAt,
            IsApproved = entity.IsApproved,
            TermInMonths = entity.TermInMonths
        };
    }

    public Credit Map(CreditDto dto)
    {
        return new Credit
        {
            Id = dto.Id,
            Name = dto.Name,
            Amount = dto.Amount,
            InterestRate = dto.InterestRate,
            ClosedAt = dto.ClosedAt,
            CreatedAt = dto.CreatedAt,
            LastUpdatedAt = dto.LastUpdatedAt,
            IsApproved = dto.IsApproved,
            TermInMonths = dto.TermInMonths
        };
    }
}