using Domain.Dtos.BankClientDtos;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Domain.Interfaces;

namespace Domain.Mappers;

public class CompanyMapper : IMapper<Company, CompanyDto>
{
    public CompanyDto Map(Company entity)
    {
        return new CompanyDto
        {
            Id = entity.Id,
            Address = entity.Address,
            CompanyType = entity.CompanyType,
            IsApproved = entity.IsApproved,
            RecordIds = entity.Records.Select(record => record.Id).ToList(),
            ServiceIds = entity.Services.Select(service => service.Id).ToList(),
            TaxIdentificationNumber = entity.TaxIdentificationNumber,
            TaxIdentificationType = entity.TaxIdentificationType
        };
    }

    public Company Map(CompanyDto dto)
    {
        return new Company
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
            CompanyType = dto.CompanyType,
            IsApproved = dto.IsApproved,
            TaxIdentificationNumber = dto.TaxIdentificationNumber,
            TaxIdentificationType = dto.TaxIdentificationType
        };
    }
}