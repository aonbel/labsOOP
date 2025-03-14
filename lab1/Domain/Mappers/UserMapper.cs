using Domain.Dtos;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Interfaces;

namespace Domain.Mappers;

public class UserMapper : IMapper<User, UserDto>
{
    public UserDto Map(User entity)
    {
        var dto = new UserDto
        {
            Id = entity.Id,
            Login = entity.Login,
            Password = entity.Password,
            Role = entity.Role,
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

    public User Map(UserDto dto)
    {
        var entity = new User
        {
            Id = dto.Id,
            Login = dto.Login,
            Password = dto.Password,
            Role = dto.Role
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