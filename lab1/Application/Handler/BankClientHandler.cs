using Application.Interfaces;
using Domain.Entities.BankClients;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Mappers;

namespace Application.Handler;

public class BankClientHandler(
    IClientRepository clientRepository,
    ICompanyRepository companyRepository,
    ICompanyEmployeeRepository companyEmployeeRepository,
    IBankRecordHandler bankRecordHandler,
    IBankServiceHandler bankServiceHandler,
    IMapper<Client, ClientDto> clientMapper,
    IMapper<Company, CompanyDto> companyMapper,
    IMapper<CompanyEmployee, CompanyEmployeeDto> companyEmployeeMapper) : IBankClientHandler
{
    public async Task<int> CreateClientAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        return bankClient switch
        {
            CompanyEmployee companyEmployee => await companyEmployeeRepository.AddAsync(
                companyEmployeeMapper.Map(companyEmployee), cancellationToken),
            Company company => await companyRepository.AddAsync(companyMapper.Map(company), cancellationToken),
            Client client => await clientRepository.AddAsync(clientMapper.Map(client), cancellationToken),
            _ => throw new ArgumentException(null, nameof(bankClient))
        };
    }

    public async Task UpdateClientByIdAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        await (bankClient switch
        {
            CompanyEmployee companyEmployee => companyEmployeeRepository.UpdateAsync(
                companyEmployeeMapper.Map(companyEmployee), cancellationToken),
            Company company => companyRepository.UpdateAsync(companyMapper.Map(company), cancellationToken),
            Client client => clientRepository.UpdateAsync(clientMapper.Map(client), cancellationToken),
            _ => throw new ArgumentException(null, nameof(bankClient))
        });
    }

    public async Task<BankClient> GetClientInfoByIdAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        switch (bankClient)
        {
            case CompanyEmployee companyEmployee:
            {
                var companyEmployeeDto =
                    await companyEmployeeRepository.GetByIdAsync(companyEmployee.Id, cancellationToken);

                return companyEmployeeMapper.Map(companyEmployeeDto);
            }
            case Company company:
            {
                var companyDto = await companyRepository.GetByIdAsync(company.Id, cancellationToken);

                return companyMapper.Map(companyDto);
            }
            case Client client:
            {
                var clientDto = await clientRepository.GetByIdAsync(client.Id, cancellationToken);

                return clientMapper.Map(clientDto);
            }
            default:
                throw new ArgumentException(null, nameof(bankClient));
        }
    }

    public async Task<BankClient> GetClientServicesAndRecordsInfoByIdAsync(BankClient bankClient,
        CancellationToken cancellationToken)
    {
        bankClient.Records =
            await bankRecordHandler.GetBankRecordsInfoByBankClientIdAsync(bankClient, cancellationToken);
        bankClient.Services =
            await bankServiceHandler.GetBankServicesInfoByBankClientIdAsync(bankClient, cancellationToken);

        return bankClient;
    }

    public async Task<BankClient> GetClientByIdAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        bankClient.Records =
            await bankRecordHandler.GetBankRecordsInfoByBankClientIdAsync(bankClient, cancellationToken);
        bankClient.Services =
            await bankServiceHandler.GetBankServicesInfoByBankClientIdAsync(bankClient, cancellationToken);

        switch (bankClient)
        {
            case CompanyEmployee companyEmployee:
            {
                var companyEmployeeDto =
                    await companyEmployeeRepository.GetByIdAsync(companyEmployee.Id, cancellationToken);

                return companyEmployeeMapper.Map(companyEmployeeDto);
            }
            case Company company:
            {
                var companyDto = await companyRepository.GetByIdAsync(company.Id, cancellationToken);

                return companyMapper.Map(companyDto);
            }
            case Client client:
            {
                var clientDto = await clientRepository.GetByIdAsync(client.Id, cancellationToken);

                return clientMapper.Map(clientDto);
            }
            default:
                throw new ArgumentException(null, nameof(bankClient));
        }
    }

    public async Task<ICollection<CompanyEmployee>> GetCompanyEmployeesInfoBySalaryProjectIdAsync(int salaryProjectId,
        CancellationToken cancellationToken)
    {
        var companyEmployeeDtos =
            await companyEmployeeRepository.GetCompanyEmployeesBySalaryProjectIdAsync(salaryProjectId,
                cancellationToken);

        return companyEmployeeDtos.Select(companyEmployeeMapper.Map).ToList();
    }

    public async Task DeleteClientByIdAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        switch (bankClient)
        {
            case CompanyEmployee:
                await companyEmployeeRepository.DeleteAsync(bankClient.Id, cancellationToken);
                break;
            case Company:
                await companyRepository.DeleteAsync(bankClient.Id, cancellationToken);
                break;
            case Client:
                await clientRepository.DeleteAsync(bankClient.Id, cancellationToken);
                break;
            default:
                throw new ArgumentException(null, nameof(bankClient));
        }
    }
}