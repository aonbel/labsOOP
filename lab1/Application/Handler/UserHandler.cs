using System.Transactions;
using Application.Interfaces;
using Domain.Entities.BankClients;
using Domain.Entities;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;

namespace Application.Handler;

public class UserHandler(
    IUserRepository userRepository,
    IBankClientHandler clientHandler) : IUserHandler
{
    public async Task<int> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        var userDto = new UserDto
        {
            Login = user.Login,
            Password = user.Password,
            Role = user.Role
        };

        switch (user.BankClient)
        {
            case CompanyEmployee companyEmployee:
                userDto.CompanyEmployeeId = companyEmployee.Id;
                break;
            case Company company:
                userDto.CompanyId = company.Id;
                break;
            case Client client:
                userDto.ClientId = client.Id;
                break;
        }

        return await userRepository.AddAsync(userDto, cancellationToken);
    }

    public async Task<User> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        using var transactionScope = new TransactionScope();

        var userDto = await userRepository.GetByIdAsync(id, cancellationToken);

        var user = new User
        {
            Id = userDto.Id,
            Login = userDto.Login,
            Password = userDto.Password,
            Role = userDto.Role
        };

        if (userDto.CompanyEmployeeId.HasValue)
        {
            user.BankClient =
                await clientHandler.GetClientByIdAsync(new CompanyEmployee
                {
                    Id = userDto.CompanyEmployeeId.Value
                }, cancellationToken);
        }

        if (userDto.CompanyId.HasValue)
        {
            user.BankClient =
                await clientHandler.GetClientByIdAsync(new Company
                {
                    Id = userDto.CompanyId.Value
                }, cancellationToken);
        }

        if (userDto.ClientId.HasValue)
        {
            user.BankClient =
                await clientHandler.GetClientByIdAsync(new Client
                {
                    Id = userDto.ClientId.Value
                }, cancellationToken);
        }

        transactionScope.Complete();

        return user;
    }

    public async Task<User> GetUserByLoginAsync(string login, CancellationToken cancellationToken)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var userDto = await userRepository.GetByLoginAsync(login, cancellationToken);

        var user = new User
        {
            Id = userDto.Id,
            Login = userDto.Login,
            Password = userDto.Password,
            Role = userDto.Role
        };

        if (userDto.CompanyEmployeeId.HasValue)
        {
            user.BankClient =
                await clientHandler.GetClientByIdAsync(new CompanyEmployee
                {
                    Id = userDto.CompanyEmployeeId.Value
                }, cancellationToken);
        }

        if (userDto.CompanyId.HasValue)
        {
            user.BankClient =
                await clientHandler.GetClientByIdAsync(new Company
                {
                    Id = userDto.CompanyId.Value
                }, cancellationToken);
        }

        if (userDto.ClientId.HasValue)
        {
            user.BankClient =
                await clientHandler.GetClientByIdAsync(new Client
                {
                    Id = userDto.ClientId.Value
                }, cancellationToken);
        }

        transactionScope.Complete();

        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var userDto = new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Password = user.Password,
            Role = user.Role
        };

        switch (user.BankClient)
        {
            case CompanyEmployee companyEmployee:
                userDto.CompanyEmployeeId = companyEmployee.Id;
                break;
            case Company company:
                userDto.CompanyId = company.Id;
                break;
            case Client client:
                userDto.ClientId = client.Id;
                break;
        }
        
        await userRepository.UpdateAsync(userDto, cancellationToken);
    }
}