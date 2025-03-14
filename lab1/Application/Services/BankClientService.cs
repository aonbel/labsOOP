using Application.Interfaces;
using Domain.Dtos.BankClientDtos;
using Domain.Entities.BankClients;
using Domain.Interfaces;
using Domain.Interfaces.IRepositories.IBankClientRepositories;

namespace Application.Services;

public class BankClientService(
    IClientRepository clientRepository,
    ICompanyRepository companyRepository,
    ICompanyEmployeeRepository companyEmployeeRepository,
    IBankRecordService bankRecordService,
    IMapper<Client, ClientDto> clientMapper,
    IMapper<Company, CompanyDto> companyMapper,
    IMapper<CompanyEmployee, CompanyEmployeeDto> companyEmployeeMapper) : IBankClientService
{
    public IBankServiceService bankServiceService;
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
            await bankRecordService.GetBankRecordsInfoByBankClientIdAsync(bankClient, cancellationToken);
        bankClient.Services =
            await bankServiceService.GetBankServicesInfoByBankClientIdAsync(bankClient, cancellationToken);

        return bankClient;
    }

    public async Task<BankClient> GetClientByIdAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        BankClient? result = bankClient switch
        {
            CompanyEmployee companyEmployee => companyEmployeeMapper.Map(
                await companyEmployeeRepository.GetByIdAsync(companyEmployee.Id, cancellationToken)),
            Company company => companyMapper.Map(await companyRepository.GetByIdAsync(company.Id, cancellationToken)),
            Client client => clientMapper.Map(await clientRepository.GetByIdAsync(client.Id, cancellationToken)),
            _ => throw new ArgumentException(null, nameof(bankClient))
        };

        result.Records =
            await bankRecordService.GetBankRecordsInfoByBankClientIdAsync(bankClient, cancellationToken);
        result.Services =
            await bankServiceService.GetBankServicesInfoByBankClientIdAsync(bankClient, cancellationToken);
        
        return result;
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