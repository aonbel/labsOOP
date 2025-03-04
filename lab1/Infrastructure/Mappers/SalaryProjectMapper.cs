using Domain.Entities.BankServices;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;

namespace Infrastructure.Mappers;

public class SalaryProjectMapper : IMapper<SalaryProject, SalaryProjectDto>
{
    public SalaryProjectDto Map(SalaryProject entity)
    {
        return new SalaryProjectDto
        {
            Id = entity.Id,
            Name = entity.Name,
            BankRecordId = entity.Record.Id,
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
            Name = dto.Name,
            CreatedAt = dto.CreatedAt,
            ClosedAt = dto.ClosedAt,
            LastUpdatedAt = dto.LastUpdatedAt,
            IsApproved = dto.IsApproved,
            TermInMonths = dto.TermInMonths
        };
    }
}