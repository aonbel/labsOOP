using Domain.Dtos;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Interfaces;

namespace Domain.Mappers;

public class BankRecordMapper : IMapper<BankRecord, BankRecordDto>
{
    public BankRecordDto Map(BankRecord entity)
    {
        var dto = new BankRecordDto
        {
            Id = entity.Id,
            Amount = entity.Amount,
            BankId = entity.Bank?.Id ?? 0,
            IsActive = entity.IsActive
        };

        switch (entity.BankClient)
        {
            case CompanyEmployee companyEmployee:
                dto.CompanyEmployeeId = companyEmployee.Id;
                break;
            case Company company:
                dto.CompanyId = company.Id;
                break;
            case Client client:
                dto.ClientId = client.Id;
                break;
        }
        
        return dto;
    }

    public BankRecord Map(BankRecordDto dto)
    {
        var entity = new BankRecord
        {
            Id = dto.Id,
            Amount = dto.Amount,
            Bank = new Bank
            {
                Id = dto.BankId
            },
            IsActive = dto.IsActive
        };

        if (dto.CompanyEmployeeId.HasValue)
        {
            entity.BankClient = new CompanyEmployee
            {
                Id = dto.CompanyEmployeeId.Value
            };
        }

        if (dto.CompanyId.HasValue)
        {
            entity.BankClient = new Company
            {
                Id = dto.CompanyId.Value
            };
        }

        if (dto.ClientId.HasValue)
        {
            entity.BankClient = new Client
            {
                Id = dto.ClientId.Value
            };
        }
        
        return entity;
    }
}