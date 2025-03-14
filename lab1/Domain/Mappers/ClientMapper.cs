using Domain.Dtos.BankClientDtos;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Domain.Interfaces;

namespace Domain.Mappers;

public class ClientMapper : IMapper<Client, ClientDto>
{
    public ClientDto Map(Client entity)
    {
        return new ClientDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            PassportNumber = entity.PassportNumber,
            PassportSeries = entity.PassportSeries,
            IdentificationNumber = entity.IdentificationNumber,
            IsApproved = entity.IsApproved
        };
    }

    public Client Map(ClientDto dto)
    {
        return new Client
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
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            PassportNumber = dto.PassportNumber,
            PassportSeries = dto.PassportSeries,
            IdentificationNumber = dto.IdentificationNumber,
            IsApproved = dto.IsApproved
        };
    }
}