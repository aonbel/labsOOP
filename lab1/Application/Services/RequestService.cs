using Application.Interfaces;
using Domain.Dtos.BankClientDtos;
using Domain.Dtos.BankServiceDtos;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Domain.Interfaces;
using Domain.Interfaces.IRepositories.IBankClientRepositories;
using Domain.Interfaces.IRepositories.IBankServiceRepositories;
using Infrastructure.Repositories.BankClientRepositories;
using Infrastructure.Repositories.BankServiceRepositories;

namespace Application.Services;

public class RequestService(
    IBankServiceService bankServiceService,
    IBankClientService bankClientService,
    IClientRepository clientRepository,
    ICompanyRepository companyRepository,
    ICompanyEmployeeRepository companyEmployeeRepository,
    ICreditRepository creditRepository,
    IDepositRepository depositRepository,
    IInstallmentRepository installmentRepository,
    ISalaryProjectRepository salaryProjectRepository,
    IMapper<Credit, CreditDto> creditMapper,
    IMapper<Deposit, DepositDto> depositMapper,
    IMapper<Installment, InstallmentDto> installmentMapper,
    IMapper<SalaryProject, SalaryProjectDto> salaryProjectMapper,
    IMapper<Client, ClientDto> clientMapper,
    IMapper<Company, CompanyDto> companyMapper,
    IMapper<CompanyEmployee, CompanyEmployeeDto> companyEmployeeMapper) : IRequestService
{
    public async Task<int> CreateBankServiceRequestAsync(int bankId, BankClient bankClient, Domain.Entities.BankServices.BankService bankService,
        CancellationToken cancellationToken)
    {
        bankService.IsApproved = false;
        return await bankServiceService.CreateBankService(bankId, bankClient, bankService, cancellationToken);
    }

    public async Task<ICollection<Domain.Entities.BankServices.BankService>> GetBankServiceRequestsAsync(CancellationToken cancellationToken)
    {
        var services = new List<Domain.Entities.BankServices.BankService>();

        services.AddRange(
            (await creditRepository.GetAllNotApprovedAsync(cancellationToken))
            .Select(Domain.Entities.BankServices.BankService (creditDto) => creditMapper.Map(creditDto))
            .ToList());

        services.AddRange(
            (await depositRepository.GetAllNotApprovedAsync(cancellationToken))
            .Select(Domain.Entities.BankServices.BankService (depositDto) => depositMapper.Map(depositDto))
            .ToList()
        );

        services.AddRange(
            (await installmentRepository.GetAllNotApprovedAsync(cancellationToken))
            .Select(Domain.Entities.BankServices.BankService (installmentDto) => installmentMapper.Map(installmentDto))
            .ToList()
        );

        services.AddRange(
            (await salaryProjectRepository.GetAllNotApprovedAsync(cancellationToken))
            .Select(Domain.Entities.BankServices.BankService (salaryProjectDto) => salaryProjectMapper.Map(salaryProjectDto))
            .ToList()
        );

        return services;
    }

    public async Task ApproveBankServiceRequestAsync(Domain.Entities.BankServices.BankService bankService, CancellationToken cancellationToken)
    {
        bankService = await bankServiceService.GetBankServiceByIdAsync(bankService, cancellationToken);
        bankService.IsApproved = true;
        await bankServiceService.UpdateBankService(bankService, cancellationToken);
    }

    public async Task DisapproveBankServiceRequestAsync(Domain.Entities.BankServices.BankService bankService, CancellationToken cancellationToken)
    {
        await bankServiceService.DeleteBankService(bankService, cancellationToken);
    }

    public async Task<int> CreateBankClientRequestAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        bankClient.IsApproved = false;
        return await bankClientService.CreateClientAsync(bankClient, cancellationToken);
    }

    public async Task<ICollection<BankClient>> GetBankClientRequestsAsync(CancellationToken cancellationToken)
    {
        var requests = new List<BankClient>();
        
        requests.AddRange((await clientRepository.GetAllNotApprovedAsync(cancellationToken))
            .Select(BankClient (bankClientDto) => clientMapper.Map(bankClientDto))
            .ToList());
        
        requests.AddRange((await companyRepository.GetAllNotApprovedAsync(cancellationToken))
            .Select(BankClient (bankClientDto) => companyMapper.Map(bankClientDto))
            .ToList());
        
        requests.AddRange((await companyEmployeeRepository.GetAllNotApprovedAsync(cancellationToken))
            .Select(BankClient (bankClientDto) => companyEmployeeMapper.Map(bankClientDto))
            .ToList());
        
        return requests;
    }

    public async Task ApproveBankClientRequestAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        bankClient = await bankClientService.GetClientByIdAsync(bankClient, cancellationToken);
        
        bankClient.IsApproved = true;
        
        await bankClientService.UpdateClientByIdAsync(bankClient, cancellationToken);
    }

    public async Task DisapproveBankClientRequestAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        await bankClientService.DeleteClientByIdAsync(bankClient, cancellationToken);
    }
}