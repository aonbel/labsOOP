using Domain.Dtos;
using Domain.Entities;
using Domain.Entities.BankServices;
using Domain.Interfaces;

namespace Domain.Mappers;

public class BankMapper : IMapper<Bank, BankDto>
{
    public BankDto Map(Bank entity)
    {
        return new BankDto
        {
            Id = entity.Id,
            Address = entity.Address,
            BankIdentificationCode = entity.BankIdentificationCode,
            CompanyType = entity.CompanyType,
            IsApproved = entity.IsApproved,
            RecordIds = entity.Records.Select(record => record.Id).ToList(),
            ServiceIds = entity.Services.Select(service => service.Id).ToList(),
            TaxIdentificationNumber = entity.TaxIdentificationNumber,
            TaxIdentificationType = entity.TaxIdentificationType
        };
    }

    public Bank Map(BankDto dto)
    {
        return new Bank
        {
            Id = dto.Id,
            Records = dto.RecordIds.Select(recordId => new BankRecord
            {
                Id = recordId
            }).ToList(),
            Services = dto.ServiceIds.Select(serviceId => new BankService
            {
                Id = serviceId
            }).ToList(),
            Address = dto.Address,
            BankIdentificationCode = dto.BankIdentificationCode,
            CompanyType = dto.CompanyType,
            IsApproved = dto.IsApproved,
            TaxIdentificationNumber = dto.TaxIdentificationNumber,
            TaxIdentificationType = dto.TaxIdentificationType
        };
    }
}