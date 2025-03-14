using System.Transactions;
using Application.Interfaces;
using Domain.Dtos;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Interfaces;
using Domain.Interfaces.IRepositories;

namespace Application.Services;

public class UserService(
    IUserRepository userRepository,
    IBankClientService clientService,
    IMapper<User, UserDto> userMapper) : IUserService
{
    public async Task<int> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        var userDto = userMapper.Map(user);

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

        var user = userMapper.Map(userDto);

        if (userDto.CompanyEmployeeId.HasValue)
        {
            user.BankClient =
                await clientService.GetClientByIdAsync(new CompanyEmployee
                {
                    Id = userDto.CompanyEmployeeId.Value
                }, cancellationToken);
        }

        if (userDto.CompanyId.HasValue)
        {
            user.BankClient =
                await clientService.GetClientByIdAsync(new Company
                {
                    Id = userDto.CompanyId.Value
                }, cancellationToken);
        }

        if (userDto.ClientId.HasValue)
        {
            user.BankClient =
                await clientService.GetClientByIdAsync(new Client
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

        var user = userMapper.Map(userDto);

        if (user.BankClient is not null)
        {
            user.BankClient = await clientService.GetClientByIdAsync(user.BankClient, cancellationToken);
        }

        transactionScope.Complete();

        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var userDto = userMapper.Map(user);

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