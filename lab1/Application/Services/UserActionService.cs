using Application.Interfaces;
using Application.Reverters;
using Domain.Dtos;
using Domain.Dtos.BankClientDtos;
using Domain.Dtos.BankServiceDtos;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Domain.Entities.Core;
using Domain.Interfaces;
using Domain.Interfaces.IRepositories;
using Domain.Interfaces.IRepositories.IBankClientRepositories;
using Domain.Interfaces.IRepositories.IBankServiceRepositories;

namespace Application.Services;

public class UserActionService(
    IUserActionRepository userActionRepository,
    ICRRepository<CreditDto> creditStateRepository,
    ICRRepository<DepositDto> depositStateRepository,
    ICRRepository<InstallmentDto> installmentStateRepository,
    ICRRepository<SalaryProjectDto> salaryProjectStateRepository,
    ICRRepository<ClientDto> clientStateRepository,
    ICRRepository<CompanyDto> companyStateRepository,
    ICRRepository<CompanyEmployeeDto> companyEmployeeStateRepository,
    ICRRepository<BankRecordDto> bankRecordStateRepository,
    ICRRepository<TransactionDto> transactionStateRepository,
    ICreditRepository creditRepository,
    IDepositRepository depositRepository,
    IInstallmentRepository installmentRepository,
    ISalaryProjectRepository salaryProjectRepository,
    IClientRepository clientRepository,
    ICompanyRepository companyRepository,
    ICompanyEmployeeRepository companyEmployeeRepository,
    IBankRecordRepository bankRecordRepository,
    ITransactionRepository transactionRepository,
    IMapper<UserAction, UserActionDto> userActionMapper,
    IMapper<Client, ClientDto> clientMapper,
    IMapper<Company, CompanyDto> companyMapper,
    IMapper<CompanyEmployee, CompanyEmployeeDto> companyEmployeeMapper,
    IMapper<Credit, CreditDto> creditMapper,
    IMapper<Deposit, DepositDto> depositMapper,
    IMapper<Installment, InstallmentDto> installmentMapper,
    IMapper<SalaryProject, SalaryProjectDto> salaryProjectMapper,
    IMapper<BankRecord, BankRecordDto> bankRecordMapper,
    IMapper<Transaction, TransactionDto> transactionMapper,
    IReverter<Credit> creditReverter,
    IReverter<Deposit> depositReverter,
    IReverter<Installment> installmentReverter,
    IReverter<SalaryProject> salaryProjectReverter,
    IReverter<Client> clientReverter,
    IReverter<Company> companyReverter,
    IReverter<CompanyEmployee> companyEmployeeReverter,
    IReverter<BankRecord> bankRecordReverter,
    IReverter<Transaction> transactionReverter) : IUserActionService
{
    public async Task<int> AddUserActionAsync(
        int userId,
        BaseEntity actionTargetStateBeforeAction,
        string actionName,
        ActionType actionType,
        CancellationToken cancellationToken)
    {
        var actionDto = new UserActionDto
        {
            Name = actionName,
            UserId = userId,
            Date = DateTime.Now,
            ActionTargetType = actionTargetStateBeforeAction.GetType().Name,
            PreviousStateId = actionTargetStateBeforeAction switch
            {
                Credit credit => await creditStateRepository.AddAsync(creditMapper.Map(credit), cancellationToken),
                Deposit deposit => await depositStateRepository.AddAsync(depositMapper.Map(deposit), cancellationToken),
                Installment installment => await installmentStateRepository.AddAsync(installmentMapper.Map(installment),
                    cancellationToken),
                SalaryProject salaryProject => await salaryProjectStateRepository.AddAsync(
                    salaryProjectMapper.Map(salaryProject), cancellationToken),
                CompanyEmployee companyEmployee => await companyEmployeeStateRepository.AddAsync(
                    companyEmployeeMapper.Map(companyEmployee), cancellationToken),
                Client client => await clientStateRepository.AddAsync(clientMapper.Map(client), cancellationToken),
                Company company => await companyStateRepository.AddAsync(companyMapper.Map(company), cancellationToken),
                Transaction transaction => await transactionStateRepository.AddAsync(transactionMapper.Map(transaction),
                    cancellationToken),
                BankRecord bankRecord => await bankRecordStateRepository.AddAsync(bankRecordMapper.Map(bankRecord),
                    cancellationToken),
                _ => throw new NotImplementedException()
            },
            Type = (int)actionType
        };

        return await userActionRepository.AddAsync(actionDto, cancellationToken);
    }

    public async Task<BaseEntity> GetCurrentStateOfActionTargetAsync(
        int userActionId,
        CancellationToken cancellationToken)
    {
        var userAction = userActionMapper.Map(await userActionRepository.GetByIdAsync(userActionId, cancellationToken));

        BaseEntity actionTarget = userAction.PreviousState switch
        {
            Credit => creditMapper.Map(
                await creditRepository.GetByIdAsync(userAction.PreviousState.Id, cancellationToken)),
            Deposit => depositMapper.Map(
                await depositRepository.GetByIdAsync(userAction.PreviousState.Id, cancellationToken)),
            Installment => installmentMapper.Map(
                await installmentRepository.GetByIdAsync(userAction.PreviousState.Id, cancellationToken)),
            SalaryProject => salaryProjectMapper.Map(
                await salaryProjectRepository.GetByIdAsync(userAction.PreviousState.Id, cancellationToken)),
            CompanyEmployee => companyEmployeeMapper.Map(
                await companyEmployeeRepository.GetByIdAsync(userAction.PreviousState.Id, cancellationToken)),
            Company => companyMapper.Map(
                await companyRepository.GetByIdAsync(userAction.PreviousState.Id, cancellationToken)),
            Client => clientMapper.Map(
                await clientRepository.GetByIdAsync(userAction.PreviousState.Id, cancellationToken)),
            BankRecord => bankRecordMapper.Map(
                await bankRecordRepository.GetByIdAsync(userAction.PreviousState.Id, cancellationToken)),
            Transaction => transactionMapper.Map(
                await transactionRepository.GetByIdAsync(userAction.PreviousState.Id, cancellationToken)),
            _ => throw new NotImplementedException()
        };
        
        return actionTarget;
    }

    public async Task<UserAction> GetUserActionByIdAsync(int userActionId, CancellationToken cancellationToken)
    {
        return userActionMapper.Map(await userActionRepository.GetByIdAsync(userActionId, cancellationToken));
    }

    public async Task<ICollection<UserAction>> GetUserActionsByUserIdAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        return (await userActionRepository.GetAllActionsByUserIdAsync(userId, cancellationToken))
            .Select(userActionMapper.Map)
            .ToList();
    }

    public async Task RevertUserActionByIdAsync(int userActionId, CancellationToken cancellationToken)
    {
        var userAction = userActionMapper.Map(await userActionRepository.GetByIdAsync(userActionId, cancellationToken));

        switch (userAction.Type)
        {
            case ActionType.Create:
                switch (userAction.PreviousState)
                {
                    case Credit credit:
                        await creditReverter.RevertCreateActionAsync(credit.Id, cancellationToken);
                        break;
                    case Deposit deposit:
                        await depositReverter.RevertCreateActionAsync(deposit.Id, cancellationToken);
                        break;
                    case Installment installment:
                        await installmentReverter.RevertCreateActionAsync(installment.Id, cancellationToken);
                        break;
                    case SalaryProject salaryProject:
                        await salaryProjectReverter.RevertCreateActionAsync(salaryProject.Id, cancellationToken);
                        break;
                    case CompanyEmployee companyEmployee:
                        await companyEmployeeReverter.RevertCreateActionAsync(companyEmployee.Id, cancellationToken);
                        break;
                    case Company company:
                        await companyReverter.RevertCreateActionAsync(company.Id, cancellationToken);
                        break;
                    case Client client:
                        await clientReverter.RevertCreateActionAsync(client.Id, cancellationToken);
                        break;
                    case BankRecord bankRecord:
                        await bankRecordReverter.RevertCreateActionAsync(bankRecord.Id, cancellationToken);
                        break;
                    case Transaction transaction:
                        await transactionReverter.RevertCreateActionAsync(transaction.Id, cancellationToken);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                break;
            case ActionType.Update:
                switch (userAction.PreviousState)
                {
                    case Credit credit:
                        await creditReverter.RevertUpdateActionAsync(credit, cancellationToken);
                        break;
                    case Deposit deposit:
                        await depositReverter.RevertUpdateActionAsync(deposit, cancellationToken);
                        break;
                    case Installment installment:
                        await installmentReverter.RevertUpdateActionAsync(installment, cancellationToken);
                        break;
                    case SalaryProject salaryProject:
                        await salaryProjectReverter.RevertUpdateActionAsync(salaryProject,
                            cancellationToken);
                        break;
                    case CompanyEmployee companyEmployee:
                        await companyEmployeeReverter.RevertUpdateActionAsync(companyEmployee,
                            cancellationToken);
                        break;
                    case Company company:
                        await companyReverter.RevertUpdateActionAsync(company, cancellationToken);
                        break;
                    case Client client:
                        await clientReverter.RevertUpdateActionAsync(client, cancellationToken);
                        break;
                    case BankRecord bankRecord:
                        await bankRecordReverter.RevertUpdateActionAsync(bankRecord, cancellationToken);
                        break;
                    case Transaction transaction:
                        await transactionReverter.RevertUpdateActionAsync(transaction, cancellationToken);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                break;
            case ActionType.Delete:
                switch (userAction.PreviousState)
                {
                    case Credit credit:
                        await creditReverter.RevertDeleteActionAsync(credit, cancellationToken);
                        break;
                    case Deposit deposit:
                        await depositReverter.RevertDeleteActionAsync(deposit, cancellationToken);
                        break;
                    case Installment installment:
                        await installmentReverter.RevertDeleteActionAsync(installment, cancellationToken);
                        break;
                    case SalaryProject salaryProject:
                        await salaryProjectReverter.RevertDeleteActionAsync(salaryProject, cancellationToken);
                        break;
                    case CompanyEmployee companyEmployee:
                        await companyEmployeeReverter.RevertDeleteActionAsync(companyEmployee, cancellationToken);
                        break;
                    case Company company:
                        await companyReverter.RevertDeleteActionAsync(company, cancellationToken);
                        break;
                    case Client client:
                        await clientReverter.RevertDeleteActionAsync(client, cancellationToken);
                        break;
                    case BankRecord bankRecord:
                        await bankRecordReverter.RevertDeleteActionAsync(bankRecord, cancellationToken);
                        break;
                    case Transaction transaction:
                        await transactionReverter.RevertDeleteActionAsync(transaction, cancellationToken);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}