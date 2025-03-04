using System.Transactions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;

namespace Application.Handler;

public class BankServiceHandler(
    IBankRecordHandler bankRecordHandler,
    ICreditRepository creditRepository,
    IDepositRepository depositRepository,
    IInstallmentRepository installmentRepository,
    ISalaryProjectRepository salaryProjectRepository,
    IMapper<Credit, CreditDto> creditMapper,
    IMapper<Deposit, DepositDto> depositMapper,
    IMapper<Installment, InstallmentDto> installmentMapper,
    IMapper<SalaryProject, SalaryProjectDto> salaryProjectMapper)
    : IBankServiceHandler
{
    public async Task<int> CreateBankService(Bank bank, BankClient bankClient, BankService bankService,
        CancellationToken cancellationToken)
    {
        try
        {
            var recordId = await bankRecordHandler.CreateBankRecordAsync(bankClient, bank, cancellationToken);

            var serviceId = bankService switch
            {
                Credit credit => await creditRepository.AddAsync(creditMapper.Map(credit), cancellationToken),
                Deposit deposit => await depositRepository.AddAsync(depositMapper.Map(deposit), cancellationToken),
                Installment installment => await installmentRepository.AddAsync(installmentMapper.Map(installment), cancellationToken),
                SalaryProject salaryProject => await salaryProjectRepository.AddAsync(salaryProjectMapper.Map(salaryProject), cancellationToken),
                _ => throw new ArgumentException(null, nameof(bankService))
            };

            return serviceId;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<BankService> GetBankServiceByIdAsync(BankService bankService, CancellationToken cancellationToken)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        switch (bankService)
        {
            case Credit credit:
                var creditDto = await creditRepository.GetByIdAsync(credit.Id, cancellationToken);
                credit = creditMapper.Map(creditDto);
                credit.Record = await bankRecordHandler.GetBankRecordByIdAsync(creditDto.BankRecordId, cancellationToken);

                transactionScope.Complete();

                return credit;
            case Deposit deposit:
                var depositDto = await depositRepository.GetByIdAsync(deposit.Id, cancellationToken);
                deposit = depositMapper.Map(depositDto);
                deposit.Record = await bankRecordHandler.GetBankRecordByIdAsync(depositDto.BankRecordId, cancellationToken);

                transactionScope.Complete();

                return deposit;
            case Installment installment:
                var installmentDto = await installmentRepository.GetByIdAsync(installment.Id, cancellationToken);
                installment = installmentMapper.Map(installmentDto);
                installment.Record = await bankRecordHandler.GetBankRecordByIdAsync(installmentDto.BankRecordId, cancellationToken);

                transactionScope.Complete();

                return installment;
            case SalaryProject salaryProject:
                var salaryProjectDto = await salaryProjectRepository.GetByIdAsync(salaryProject.Id, cancellationToken);
                salaryProject = salaryProjectMapper.Map(salaryProjectDto);
                salaryProject.Record = await bankRecordHandler.GetBankRecordByIdAsync(salaryProjectDto.BankRecordId, cancellationToken);

                transactionScope.Complete();

                return salaryProject;
            default:
                throw new ArgumentException(null, nameof(bankService));
        }
    }

    public async Task<ICollection<BankService>> GetBankServicesInfoByBankClientIdAsync(BankClient bankClient,
        CancellationToken cancellationToken)
    {
        var bankRecordDtos =
            await bankRecordHandler.GetBankRecordsInfoByBankClientIdAsync(bankClient, cancellationToken);

        var bankServices = new List<BankService>();

        foreach (var bankRecord in bankRecordDtos)
        {
            foreach (var creditDto in await creditRepository.GetByBankRecordIdAsync(bankRecord.Id, cancellationToken))
            {
                bankServices.Add(creditMapper.Map(creditDto));
            }

            foreach (var depositDto in await depositRepository.GetByBankRecordIdAsync(bankRecord.Id, cancellationToken))
            {
                bankServices.Add(depositMapper.Map(depositDto));
            }

            foreach (var installmentDto in await installmentRepository.GetByBankRecordIdAsync(bankRecord.Id,
                         cancellationToken))
            {
                bankServices.Add(installmentMapper.Map(installmentDto));
            }

            foreach (var salaryProjectDto in await salaryProjectRepository.GetByBankRecordIdAsync(bankRecord.Id,
                         cancellationToken))
            {
                bankServices.Add(salaryProjectMapper.Map(salaryProjectDto));
            }
        }

        return bankServices;
    }

    public async Task UpdateBankService(BankService service, CancellationToken cancellationToken)
    {
        switch (service)
        {
            case Credit credit:
                await creditRepository.UpdateAsync(creditMapper.Map(credit), cancellationToken);
                break;
            case Deposit deposit:
                await depositRepository.UpdateAsync(depositMapper.Map(deposit), cancellationToken);
                break;
            case Installment installment:
                await installmentRepository.UpdateAsync(installmentMapper.Map(installment), cancellationToken);
                break;
            case SalaryProject salaryProject:
                await salaryProjectRepository.UpdateAsync(salaryProjectMapper.Map(salaryProject), cancellationToken);
                break;
            default:
                throw new ArgumentException(null, nameof(service));
        }
    }

    public async Task DeleteBankService(BankService service, CancellationToken cancellationToken)
    {
        switch (service)
        {
            case Credit:
                await creditRepository.DeleteAsync(service.Id, cancellationToken);
                break;
            case Deposit:
                await depositRepository.DeleteAsync(service.Id, cancellationToken);
                break;
            case Installment:
                await installmentRepository.DeleteAsync(service.Id, cancellationToken);
                break;
        }
    }
}