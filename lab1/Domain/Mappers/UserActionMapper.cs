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
            PreviousState = dto.ActionTargetType switch
            {
                nameof(CreditDto) => new Credit(),
                nameof(DepositDto) => new Deposit(),
                nameof(InstallmentDto) => new Installment(),
                nameof(SalaryProjectDto) => new SalaryProject(),
                nameof(ClientDto) => new Client(),
                nameof(CompanyDto) => new Company(),
                nameof(CompanyEmployeeDto) => new CompanyEmployee(),
                nameof(BankRecordDto) => new BankRecord(),
                nameof(TransactionDto) => new Transaction(),
                _ => throw new ArgumentOutOfRangeException()
            },
            Type = (ActionType)dto.Type,
        };
        
        userAction.PreviousState.Id = dto.PreviousStateId;
        
        return userAction;
    }
}