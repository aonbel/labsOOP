using Domain.Dtos.BankServiceDtos;
using Domain.Entities;
using Domain.Entities.BankServices;
using Domain.Interfaces;

namespace Domain.Mappers;

public class SalaryProjectMapper : IMapper<SalaryProject, SalaryProjectDto>
{
    public SalaryProjectDto Map(SalaryProject entity)
    {
        return new SalaryProjectDto
        {
            Id = entity.Id,
            BankRecordId = entity.Record?.Id ?? 0,
            CreatedAt = entity.CreatedAt,
            ClosedAt = entity.ClosedAt,
            LastUpdatedAt = entity.LastUpdatedAt,
            IsApproved = entity.IsApproved,
            TermInMonths = entity.TermInMonths
        };
    }

    public SalaryProject Map(SalaryProjectDto dto)
    {
        return new SalaryProject
        {
            Id = dto.Id,
            Record = new BankRecord
            {
                Id = dto.BankRecordId,
            },
            CreatedAt = dto.CreatedAt,
            ClosedAt = dto.ClosedAt,
            LastUpdatedAt = dto.LastUpdatedAt,
            IsApproved = dto.IsApproved,
            TermInMonths = dto.TermInMonths
        };
    }
}