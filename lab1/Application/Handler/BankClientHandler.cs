using Application.Interfaces;
using Domain.Entities.BankClients;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;

namespace Application.Handler;

public class BankClientHandler(
    IRepository<ClientDto> clientRepository,
    IRepository<CompanyDto> companyRepository,
    ICompanyEmployeeRepository companyEmployeeRepository,
    IBankRecordHandler bankRecordHandler,
    IBankServiceHandler bankServiceHandler) : IBankClientHandler
{
    public async Task<int> CreateClientAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        return bankClient switch
        {
            CompanyEmployee companyEmployee => await companyEmployeeRepository.AddAsync(
                new CompanyEmployeeDto
                {
                    SalaryProjectId = 0,
                    Email = companyEmployee.Email,
                    FirstName = companyEmployee.FirstName,
                    LastName = companyEmployee.LastName,
                    PhoneNumber = companyEmployee.PhoneNumber,
                    IdentificationNumber = companyEmployee.IdentificationNumber,
                    PassportNumber = companyEmployee.PassportNumber,
                    PassportSeries = companyEmployee.PassportSeries,
                    Position = companyEmployee.Position,
                    Salary = companyEmployee.Salary
                }, cancellationToken),
            Company company => await companyRepository.AddAsync(
                new CompanyDto
                {
                    CompanyType = company.CompanyType,
                    TaxIdentificationNumber = company.TaxIdentificationNumber,
                    TaxIdentificationType = company.TaxIdentificationType,
                    Address = company.Address,
                }, cancellationToken),
            Client client => await clientRepository.AddAsync(
                new ClientDto
                {
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Email = client.Email,
                    PassportNumber = client.PassportNumber,
                    PassportSeries = client.PassportSeries,
                    IdentificationNumber = client.IdentificationNumber,
                    PhoneNumber = client.PhoneNumber
                }, cancellationToken),
            _ => throw new ArgumentException(null, nameof(bankClient))
        };
    }

    public async Task UpdateClientByIdAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        await (bankClient switch
        {
            CompanyEmployee companyEmployee => companyEmployeeRepository.UpdateAsync(new CompanyEmployeeDto
            {
                Id = companyEmployee.Id,
                SalaryProjectId = 0,
                Email = companyEmployee.Email,
                FirstName = companyEmployee.FirstName,
                LastName = companyEmployee.LastName,
                PhoneNumber = companyEmployee.PhoneNumber,
                IdentificationNumber = companyEmployee.IdentificationNumber,
                PassportNumber = companyEmployee.PassportNumber,
                PassportSeries = companyEmployee.PassportSeries,
                Position = companyEmployee.Position,
                Salary = companyEmployee.Salary
            }, cancellationToken),
            Company company => companyRepository.UpdateAsync(new CompanyDto
            {
                Id = company.Id,
                CompanyType = company.CompanyType,
                TaxIdentificationNumber = company.TaxIdentificationNumber,
                TaxIdentificationType = company.TaxIdentificationType,
                Address = company.Address
            }, cancellationToken),
            Client client => clientRepository.UpdateAsync(new ClientDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                PassportNumber = client.PassportNumber,
                PassportSeries = client.PassportSeries,
                IdentificationNumber = client.IdentificationNumber,
                PhoneNumber = client.PhoneNumber
            }, cancellationToken),
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

                return new CompanyEmployee
                {
                    Id = companyEmployeeDto.Id,
                    FirstName = companyEmployeeDto.FirstName,
                    LastName = companyEmployeeDto.LastName,
                    Email = companyEmployeeDto.Email,
                    PassportNumber = companyEmployeeDto.PassportNumber,
                    PassportSeries = companyEmployeeDto.PassportSeries,
                    IdentificationNumber = companyEmployeeDto.IdentificationNumber,
                    PhoneNumber = companyEmployeeDto.PhoneNumber,
                    Position = companyEmployeeDto.Position,
                    Salary = companyEmployeeDto.Salary
                };
            }
            case Company company:
            {
                var companyDto = await companyRepository.GetByIdAsync(company.Id, cancellationToken);

                return new Company
                {
                    Id = companyDto.Id,
                    CompanyType = companyDto.CompanyType,
                    TaxIdentificationNumber = companyDto.TaxIdentificationNumber,
                    TaxIdentificationType = companyDto.TaxIdentificationType,
                    Address = companyDto.Address
                };
            }
            case Client client:
            {
                var clientDto = await clientRepository.GetByIdAsync(client.Id, cancellationToken);

                return new Client
                {
                    Id = clientDto.Id,
                    FirstName = clientDto.FirstName,
                    LastName = clientDto.LastName,
                    Email = clientDto.Email,
                    PassportNumber = clientDto.PassportNumber,
                    PassportSeries = clientDto.PassportSeries,
                    IdentificationNumber = clientDto.IdentificationNumber,
                    PhoneNumber = clientDto.PhoneNumber
                };
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

                return new CompanyEmployee
                {
                    Id = companyEmployeeDto.Id,
                    FirstName = companyEmployeeDto.FirstName,
                    LastName = companyEmployeeDto.LastName,
                    Email = companyEmployeeDto.Email,
                    PassportNumber = companyEmployeeDto.PassportNumber,
                    PassportSeries = companyEmployeeDto.PassportSeries,
                    IdentificationNumber = companyEmployeeDto.IdentificationNumber,
                    PhoneNumber = companyEmployeeDto.PhoneNumber,
                    Position = companyEmployeeDto.Position,
                    Salary = companyEmployeeDto.Salary
                };
            }
            case Company company:
            {
                var companyDto = await companyRepository.GetByIdAsync(company.Id, cancellationToken);

                return new Company
                {
                    Id = companyDto.Id,
                    CompanyType = companyDto.CompanyType,
                    TaxIdentificationNumber = companyDto.TaxIdentificationNumber,
                    TaxIdentificationType = companyDto.TaxIdentificationType,
                    Address = companyDto.Address
                };
            }
            case Client client:
            {
                var clientDto = await clientRepository.GetByIdAsync(client.Id, cancellationToken);

                return new Client
                {
                    Id = clientDto.Id,
                    FirstName = clientDto.FirstName,
                    LastName = clientDto.LastName,
                    Email = clientDto.Email,
                    PassportNumber = clientDto.PassportNumber,
                    PassportSeries = clientDto.PassportSeries,
                    IdentificationNumber = clientDto.IdentificationNumber,
                    PhoneNumber = clientDto.PhoneNumber
                };
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

        return companyEmployeeDtos.Select(companyEmployeeDto => new CompanyEmployee
        {
            Id = companyEmployeeDto.Id,
            FirstName = companyEmployeeDto.FirstName,
            LastName = companyEmployeeDto.LastName,
            Email = companyEmployeeDto.Email,
            PassportNumber = companyEmployeeDto.PassportNumber,
            PassportSeries = companyEmployeeDto.PassportSeries,
            IdentificationNumber = companyEmployeeDto.IdentificationNumber,
            PhoneNumber = companyEmployeeDto.PhoneNumber
        }).ToList();
    }
}