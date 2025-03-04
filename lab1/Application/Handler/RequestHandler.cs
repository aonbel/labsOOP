using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Mappers;
using Infrastructure.Repositories;

namespace Application.Handler;

public class RequestHandler(
    IBankServiceHandler bankServiceHandler,
    IBankClientHandler bankClientHandler,
    IClientRepository clientRepository,
    ICreditRepository creditRepository,
    IDepositRepository depositRepository,
    InstallmentRepository installmentRepository,
    ISalaryProjectRepository salaryProjectRepository,
    IMapper<Credit, CreditDto> creditMapper,
    IMapper<Deposit, DepositDto> depositMapper,
    IMapper<Installment, InstallmentDto> installmentMapper,
    IMapper<SalaryProject, SalaryProjectDto> salaryProjectMapper,
    IMapper<Client, ClientDto> clientMapper) : IRequestHandler
{
    public async Task<int> CreateBankServiceRequestAsync(Bank bank, BankClient bankClient, BankService bankService,
        CancellationToken cancellationToken)
    {
        bankService.IsApproved = false;
        return await bankServiceHandler.CreateBankService(bank, bankClient, bankService, cancellationToken);
    }

    public async Task<ICollection<BankService>> GetBankServiceRequestsAsync(CancellationToken cancellationToken)
    {
        var services = new List<BankService>();

        services.AddRange(
            (await creditRepository.GetAllNotApprovedAsync(cancellationToken))
            .Select(BankService (creditDto) => creditMapper.Map(creditDto))
            .ToList());

        services.AddRange(
            (await depositRepository.GetAllNotApprovedAsync(cancellationToken))
            .Select(BankService (depositDto) => depositMapper.Map(depositDto))
            .ToList()
        );

        services.AddRange(
            (await installmentRepository.GetAllNotApprovedAsync(cancellationToken))
            .Select(BankService (installmentDto) => installmentMapper.Map(installmentDto))
            .ToList()
        );

        services.AddRange(
            (await salaryProjectRepository.GetAllNotApprovedAsync(cancellationToken))
            .Select(BankService (salaryProjectDto) => salaryProjectMapper.Map(salaryProjectDto))
            .ToList()
        );

        return services;
    }

    public async Task ApproveBankServiceRequestAsync(BankService bankService, CancellationToken cancellationToken)
    {
        var bankServiceDto = await bankServiceHandler.GetBankServiceByIdAsync(bankService, cancellationToken);
        bankService.IsApproved = true;
        await bankServiceHandler.UpdateBankService(bankServiceDto, cancellationToken);
    }

    public async Task DisapproveBankServiceRequestAsync(BankService bankService, CancellationToken cancellationToken)
    {
        await bankServiceHandler.DeleteBankService(bankService, cancellationToken);
    }

    public async Task<int> CreateBankClientRequestAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        bankClient.IsApproved = false;
        return await bankClientHandler.CreateClientAsync(bankClient, cancellationToken);
    }

    public async Task<ICollection<BankClient>> GetBankClientRequestsAsync(CancellationToken cancellationToken)
    {
        return (await clientRepository.GetAllNotApprovedAsync(cancellationToken))
            .Select(BankClient (bankClientDto) => clientMapper.Map(bankClientDto))
            .ToList();
    }

    public async Task ApproveBankClientRequestAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        bankClient = await bankClientHandler.GetClientByIdAsync(bankClient, cancellationToken);
        
        bankClient.IsApproved = true;
        
        await bankClientHandler.UpdateClientByIdAsync(bankClient, cancellationToken);
    }

    public async Task DisapproveBankClientRequestAsync(BankClient bankClient, CancellationToken cancellationToken)
    {
        await bankClientHandler.DeleteClientByIdAsync(bankClient, cancellationToken);
    }
}