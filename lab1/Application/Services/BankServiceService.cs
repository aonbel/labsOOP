using System.Transactions;
using Application.Interfaces;
using Domain.Dtos.BankServiceDtos;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Domain.Interfaces;
using Domain.Interfaces.IRepositories.IBankServiceRepositories;

namespace Application.Services;

public class BankServiceService(
    IBankRecordService bankRecordService,
    ICreditRepository creditRepository,
    IDepositRepository depositRepository,
    IInstallmentRepository installmentRepository,
    ISalaryProjectRepository salaryProjectRepository,
    IMapper<Credit, CreditDto> creditMapper,
    IMapper<Deposit, DepositDto> depositMapper,
    IMapper<Installment, InstallmentDto> installmentMapper,
    IMapper<SalaryProject, SalaryProjectDto> salaryProjectMapper)
    : IBankServiceService
{
    public async Task<int> CreateBankService(
        int bankId,
        BankClient bankClient,
        Domain.Entities.BankServices.BankService bankService,
        CancellationToken cancellationToken)
    {
        try
        {
            var recordId = await bankRecordService.CreateBankRecordAsync(bankClient, bankId, cancellationToken);

            bankService.Record = new BankRecord
            {
                Id = recordId
            };

            bankService.CreatedAt = DateTime.Now;
            bankService.LastUpdatedAt = DateTime.Now;

            var serviceId = bankService switch
            {
                Credit credit => await creditRepository.AddAsync(creditMapper.Map(credit), cancellationToken),
                Deposit deposit => await depositRepository.AddAsync(depositMapper.Map(deposit), cancellationToken),
                Installment installment => await installmentRepository.AddAsync(installmentMapper.Map(installment),
                    cancellationToken),
                SalaryProject salaryProject => await salaryProjectRepository.AddAsync(
                    salaryProjectMapper.Map(salaryProject), cancellationToken),
                _ => throw new ArgumentException(null, nameof(bankService))
            };

            return serviceId;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<int> CreateBankService(Domain.Entities.BankServices.BankService bankService,
        CancellationToken cancellationToken)
    {
        var serviceId = bankService switch
        {
            Credit credit => await creditRepository.AddAsync(creditMapper.Map(credit), cancellationToken),
            Deposit deposit => await depositRepository.AddAsync(depositMapper.Map(deposit), cancellationToken),
            Installment installment => await installmentRepository.AddAsync(installmentMapper.Map(installment),
                cancellationToken),
            SalaryProject salaryProject => await salaryProjectRepository.AddAsync(
                salaryProjectMapper.Map(salaryProject), cancellationToken),
            _ => throw new ArgumentException(null, nameof(bankService))
        };

        return serviceId;
    }

    public async Task<Domain.Entities.BankServices.BankService> GetBankServiceByIdAsync(
        Domain.Entities.BankServices.BankService bankService, CancellationToken cancellationToken)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        switch (bankService)
        {
            case Credit credit:
                var creditDto = await creditRepository.GetByIdAsync(credit.Id, cancellationToken);
                credit = creditMapper.Map(creditDto);
                credit.Record =
                    await bankRecordService.GetBankRecordByIdAsync(creditDto.BankRecordId, cancellationToken);

                transactionScope.Complete();

                return credit;
            case Deposit deposit:
                var depositDto = await depositRepository.GetByIdAsync(deposit.Id, cancellationToken);
                deposit = depositMapper.Map(depositDto);
                deposit.Record =
                    await bankRecordService.GetBankRecordByIdAsync(depositDto.BankRecordId, cancellationToken);

                transactionScope.Complete();

                return deposit;
            case Installment installment:
                var installmentDto = await installmentRepository.GetByIdAsync(installment.Id, cancellationToken);
                installment = installmentMapper.Map(installmentDto);
                installment.Record =
                    await bankRecordService.GetBankRecordByIdAsync(installmentDto.BankRecordId, cancellationToken);

                transactionScope.Complete();

                return installment;
            case SalaryProject salaryProject:
                var salaryProjectDto = await salaryProjectRepository.GetByIdAsync(salaryProject.Id, cancellationToken);
                salaryProject = salaryProjectMapper.Map(salaryProjectDto);
                salaryProject.Record =
                    await bankRecordService.GetBankRecordByIdAsync(salaryProjectDto.BankRecordId, cancellationToken);

                transactionScope.Complete();

                return salaryProject;
            default:
                throw new ArgumentException(null, nameof(bankService));
        }
    }

    public async Task<ICollection<Domain.Entities.BankServices.BankService>> GetBankServicesInfoByBankClientIdAsync(
        BankClient bankClient,
        CancellationToken cancellationToken)
    {
        var bankRecordDtos =
            await bankRecordService.GetBankRecordsInfoByBankClientIdAsync(bankClient, cancellationToken);

        var bankServices = new List<Domain.Entities.BankServices.BankService>();

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

    public async Task UpdateBankService(Domain.Entities.BankServices.BankService service,
        CancellationToken cancellationToken)
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

    public async Task DeleteBankService(Domain.Entities.BankServices.BankService service,
        CancellationToken cancellationToken)
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