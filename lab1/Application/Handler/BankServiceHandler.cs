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
    ISalaryProjectRepository salaryProjectRepository)
    : IBankServiceHandler
{
    public async Task<int> CreateBankService(BankClient bankClient, BankService bankService, Bank bank,
        CancellationToken cancellationToken)
    {
        try
        {
            var recordId = await bankRecordHandler.CreateBankRecordAsync(bankClient, bank, cancellationToken);

            var serviceId = bankService switch
            {
                Credit credit => await creditRepository.AddAsync(new CreditDto
                {
                    Name = credit.Name,
                    InterestRate = credit.InterestRate,
                    Amount = credit.Amount,
                    CreatedAt = DateTime.Now,
                    ClosedAt = credit.ClosedAt,
                    LastUpdatedAt = DateTime.Now,
                    TermInMonths = credit.TermInMonths,
                    BankRecordId = recordId,
                    IsApproved = credit.IsApproved
                }, cancellationToken),
                Deposit deposit => await depositRepository.AddAsync(new DepositDto
                {
                    Name = deposit.Name,
                    InterestRate = deposit.InterestRate,
                    IsInteractable = deposit.IsInteractable,
                    CreatedAt = DateTime.Now,
                    ClosedAt = deposit.ClosedAt,
                    LastUpdatedAt = DateTime.Now,
                    TermInMonths = deposit.TermInMonths,
                    BankRecordId = recordId,
                    IsApproved = deposit.IsApproved
                }, cancellationToken),
                Installment installment => await installmentRepository.AddAsync(new InstallmentDto
                {
                    Name = installment.Name,
                    InterestRate = installment.InterestRate,
                    Amount = installment.Amount,
                    CreatedAt = DateTime.Now,
                    ClosedAt = installment.ClosedAt,
                    LastUpdatedAt = DateTime.Now,
                    TermInMonths = installment.TermInMonths,
                    BankRecordId = recordId,
                    IsApproved = installment.IsApproved
                }, cancellationToken),
                SalaryProject salaryProject => await salaryProjectRepository.AddAsync(new SalaryProjectDto
                {
                    Name = salaryProject.Name,
                    BankRecordId = recordId,
                    EmployeeIds = salaryProject.Employees.Select(e => e.Id).ToList(),
                    CreatedAt = DateTime.Now,
                    ClosedAt = salaryProject.ClosedAt,
                    LastUpdatedAt = DateTime.Now,
                    TermInMonths = salaryProject.TermInMonths,
                    IsApproved = salaryProject.IsApproved
                }, cancellationToken),
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
                var creditRecord =
                    await bankRecordHandler.GetBankRecordByIdAsync(creditDto.BankRecordId, cancellationToken);

                transactionScope.Complete();

                return new Credit
                {
                    Id = creditDto.Id,
                    Name = creditDto.Name,
                    InterestRate = creditDto.InterestRate,
                    Amount = creditDto.Amount,
                    CreatedAt = creditDto.CreatedAt,
                    ClosedAt = creditDto.ClosedAt,
                    LastUpdatedAt = creditDto.LastUpdatedAt,
                    TermInMonths = creditDto.TermInMonths,
                    IsApproved = creditDto.IsApproved,
                    Record = creditRecord
                };
            case Deposit deposit:
                var depositDto = await depositRepository.GetByIdAsync(deposit.Id, cancellationToken);
                var depositRecord =
                    await bankRecordHandler.GetBankRecordByIdAsync(depositDto.BankRecordId, cancellationToken);

                transactionScope.Complete();

                return new Deposit
                {
                    Id = depositDto.Id,
                    Name = depositDto.Name,
                    InterestRate = depositDto.InterestRate,
                    IsInteractable = depositDto.IsInteractable,
                    CreatedAt = depositDto.CreatedAt,
                    ClosedAt = depositDto.ClosedAt,
                    LastUpdatedAt = depositDto.LastUpdatedAt,
                    TermInMonths = depositDto.TermInMonths,
                    IsApproved = depositDto.IsApproved,
                    Record = depositRecord
                };
            case Installment installment:
                var installmentDto = await installmentRepository.GetByIdAsync(installment.Id, cancellationToken);
                var installmentRecord =
                    await bankRecordHandler.GetBankRecordByIdAsync(installmentDto.BankRecordId, cancellationToken);

                transactionScope.Complete();

                return new Installment
                {
                    Id = installmentDto.Id,
                    Name = installmentDto.Name,
                    InterestRate = installmentDto.InterestRate,
                    Amount = installmentDto.Amount,
                    CreatedAt = installmentDto.CreatedAt,
                    ClosedAt = installmentDto.ClosedAt,
                    LastUpdatedAt = installmentDto.LastUpdatedAt,
                    TermInMonths = installmentDto.TermInMonths,
                    IsApproved = installmentDto.IsApproved,
                    Record = installmentRecord
                };
            case SalaryProject salaryProject:
                var salaryProjectDto = await salaryProjectRepository.GetByIdAsync(salaryProject.Id, cancellationToken);

                var salaryProjectRecord =
                    await bankRecordHandler.GetBankRecordByIdAsync(salaryProjectDto.BankRecordId, cancellationToken);

                transactionScope.Complete();

                return new SalaryProject
                {
                    Id = salaryProjectDto.Id,
                    Name = salaryProjectDto.Name,
                    Record = salaryProjectRecord,
                    ClosedAt = salaryProjectDto.ClosedAt,
                    CreatedAt = salaryProjectDto.CreatedAt,
                    LastUpdatedAt = salaryProjectDto.LastUpdatedAt,
                    TermInMonths = salaryProjectDto.TermInMonths,
                    IsApproved = salaryProjectDto.IsApproved
                };
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
                bankServices.Add(new Credit
                {
                    Id = creditDto.Id,
                    Name = creditDto.Name,
                    InterestRate = creditDto.InterestRate,
                    Amount = creditDto.Amount,
                    CreatedAt = creditDto.CreatedAt,
                    ClosedAt = creditDto.ClosedAt,
                    LastUpdatedAt = creditDto.LastUpdatedAt,
                    TermInMonths = creditDto.TermInMonths,
                    IsApproved = creditDto.IsApproved
                });
            }
            
            foreach (var depositDto in await depositRepository.GetByBankRecordIdAsync(bankRecord.Id, cancellationToken))
            {
                bankServices.Add(new Deposit
                {
                    Id = depositDto.Id,
                    Name = depositDto.Name,
                    InterestRate = depositDto.InterestRate,
                    IsInteractable = depositDto.IsInteractable,
                    CreatedAt = depositDto.CreatedAt,
                    ClosedAt = depositDto.ClosedAt,
                    LastUpdatedAt = depositDto.LastUpdatedAt,
                    TermInMonths = depositDto.TermInMonths,
                    IsApproved = depositDto.IsApproved
                });
            }
            
            foreach (var installmentDto in await installmentRepository.GetByBankRecordIdAsync(bankRecord.Id, cancellationToken))
            {
                bankServices.Add(new Installment
                {
                    Id = installmentDto.Id,
                    Name = installmentDto.Name,
                    InterestRate = installmentDto.InterestRate,
                    Amount = installmentDto.Amount,
                    CreatedAt = installmentDto.CreatedAt,
                    ClosedAt = installmentDto.ClosedAt,
                    LastUpdatedAt = installmentDto.LastUpdatedAt,
                    TermInMonths = installmentDto.TermInMonths,
                    IsApproved = installmentDto.IsApproved
                });
            }
            
            foreach (var salaryProjectDto in await salaryProjectRepository.GetByBankRecordIdAsync(bankRecord.Id, cancellationToken))
            {
                bankServices.Add(new Installment
                {
                    Id = salaryProjectDto.Id,
                    Name = salaryProjectDto.Name,
                    ClosedAt = salaryProjectDto.ClosedAt,
                    CreatedAt = salaryProjectDto.CreatedAt,
                    LastUpdatedAt = salaryProjectDto.LastUpdatedAt,
                    TermInMonths = salaryProjectDto.TermInMonths,
                    IsApproved = salaryProjectDto.IsApproved
                });
            }
        }

        return bankServices;
    }

    public async Task UpdateBankService(BankService service, CancellationToken cancellationToken)
    {
        switch (service)
        {
            case Credit credit:
                await creditRepository.UpdateAsync(new CreditDto
                {
                    Id = credit.Id,
                    Name = credit.Name,
                    BankRecordId = credit.Record.Id,
                    InterestRate = credit.InterestRate,
                    Amount = credit.Amount,
                    CreatedAt = credit.CreatedAt,
                    ClosedAt = credit.ClosedAt,
                    LastUpdatedAt = credit.LastUpdatedAt,
                    TermInMonths = credit.TermInMonths
                }, cancellationToken);
                break;
            case Deposit deposit:
                await depositRepository.UpdateAsync(new DepositDto
                {
                    Id = deposit.Id,
                    Name = deposit.Name,
                    BankRecordId = deposit.Record.Id,
                    InterestRate = deposit.InterestRate,
                    IsInteractable = deposit.IsInteractable,
                    CreatedAt = deposit.CreatedAt,
                    ClosedAt = deposit.ClosedAt,
                    LastUpdatedAt = deposit.LastUpdatedAt,
                    TermInMonths = deposit.TermInMonths,
                }, cancellationToken);
                break;
            case Installment installment:
                await installmentRepository.UpdateAsync(new InstallmentDto
                {
                    Id = installment.Id,
                    Name = installment.Name,
                    BankRecordId = installment.Record.Id,
                    InterestRate = installment.InterestRate,
                    Amount = installment.Amount,
                    CreatedAt = installment.CreatedAt,
                    ClosedAt = installment.ClosedAt,
                    LastUpdatedAt = installment.LastUpdatedAt,
                    TermInMonths = installment.TermInMonths
                }, cancellationToken);
                break;
            case SalaryProject salaryProject:
                await salaryProjectRepository.UpdateAsync(new SalaryProjectDto
                {
                    Id = salaryProject.Id,
                    Name = salaryProject.Name,
                    BankRecordId = salaryProject.Record.Id,
                    CreatedAt = salaryProject.CreatedAt,
                    ClosedAt = salaryProject.ClosedAt,
                    LastUpdatedAt = salaryProject.LastUpdatedAt,
                    TermInMonths = salaryProject.TermInMonths,
                }, cancellationToken);
                break;
            default:
                throw new ArgumentException(null, nameof(service));
        }
    }
}