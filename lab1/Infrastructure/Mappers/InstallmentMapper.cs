using Domain.Entities.BankServices;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;

namespace Infrastructure.Mappers;

public class InstallmentMapper : IMapper<Installment, InstallmentDto>
{
    public InstallmentDto Map(Installment entity)
    {
        return new InstallmentDto
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

    public Installment Map(InstallmentDto dto)
    {
        return new Installment
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