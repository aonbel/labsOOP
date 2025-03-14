using Domain.Dtos.BankServiceDtos;
using Domain.Entities;
using Domain.Entities.BankServices;
using Domain.Interfaces;

namespace Domain.Mappers;

public class InstallmentMapper : IMapper<Installment, InstallmentDto>
{
    public InstallmentDto Map(Installment entity)
    {
        return new InstallmentDto
        {
            Id = entity.Id,
            BankRecordId = entity.Record?.Id ?? 0,
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
            Record = new BankRecord
            {
                Id = dto.BankRecordId,
            },
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