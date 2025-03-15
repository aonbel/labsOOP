using Domain.Dtos;
using Domain.Dtos.BankClientDtos;
using Domain.Dtos.BankServiceDtos;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Domain.Interfaces;

namespace Domain.Mappers;

public class UserActionMapper : IMapper<UserAction, UserActionDto>
{
    public UserActionDto Map(UserAction entity)
    {
        return new UserActionDto
        {
            Id = entity.Id,
            Name = entity.Name,
            UserId = entity.User.Id,
            Date = entity.Date,
            ActionTargetType = nameof(entity.PreviousState),
            PreviousStateId = entity.Id,
            Type = (int)entity.Type
        };
    }

    public UserAction Map(UserActionDto dto)
    {
        var userAction = new UserAction
        {
            Id = dto.Id,
            Name = dto.Name,
            User = new User
            {
                Id = dto.UserId,
            },
            Date = dto.Date,
            PreviousStateId = dto.PreviousStateId,
            PreviousState = dto.ActionTargetType switch
            {
                nameof(Credit) => new Credit(),
                nameof(Deposit) => new Deposit(),
                nameof(Installment) => new Installment(),
                nameof(SalaryProject) => new SalaryProject(),
                nameof(Client) => new Client(),
                nameof(Company) => new Company(),
                nameof(CompanyEmployee) => new CompanyEmployee(),
                nameof(BankRecord) => new BankRecord(),
                nameof(Transaction) => new Transaction(),
                _ => throw new ArgumentOutOfRangeException()
            },
            Type = (ActionType)dto.Type
        };
        
        return userAction;
    }
}