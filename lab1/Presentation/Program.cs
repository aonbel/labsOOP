using Application.Interfaces;
using Application.Reverters;
using Application.Services;
using Domain.Dtos;
using Domain.Dtos.BankClientDtos;
using Domain.Dtos.BankServiceDtos;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Domain.Interfaces;
using Domain.Interfaces.IRepositories;
using Domain.Interfaces.IRepositories.IBankClientRepositories;
using Domain.Interfaces.IRepositories.IBankServiceRepositories;
using Domain.Mappers;
using Infrastructure.Options;
using Infrastructure.Repositories;
using Infrastructure.Repositories.BankClientRepositories;
using Infrastructure.Repositories.BankServiceRepositories;
using Infrastructure.Repositories.StateRepositories;
using Infrastructure.Repositories.StateRepositories.BankClientStateRepositories;
using Infrastructure.Repositories.StateRepositories.BankServiceStateRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Presentation;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((_, services) =>
    {
        services
            .AddScoped<IBankClientService, BankClientService>()
            .AddScoped<IBankService, Application.Services.BankService>()
            .AddScoped<IBankRecordService, BankRecordService>()
            .AddScoped<IBankServiceService, BankServiceService>()
            .AddScoped<ITransactionService, TransactionService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IRequestService, RequestService>()
            .AddScoped<IUserActionService, UserActionService>()
            .AddScoped<IBankRecordRepository, BankRecordRepository>()
            .AddScoped<IClientRepository, ClientRepository>()
            .AddScoped<ICompanyRepository, CompanyRepository>()
            .AddScoped<ICompanyEmployeeRepository, CompanyEmployeeRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ITransactionRepository, TransactionRepository>()
            .AddScoped<ICreditRepository, CreditRepository>()
            .AddScoped<IDepositRepository, DepositRepository>()
            .AddScoped<IInstallmentRepository, InstallmentRepository>()
            .AddScoped<ISalaryProjectRepository, SalaryProjectRepository>()
            .AddScoped<IUserActionRepository, UserActionRepository>()
            .AddScoped<IRepository<BankDto>, BankRepository>()
            .AddScoped<IRepository<ClientDto>, ClientRepository>()
            .AddScoped<IRepository<CompanyDto>, CompanyRepository>()
            .AddScoped<ICRRepository<CreditDto>, CreditStateRepository>()
            .AddScoped<ICRRepository<DepositDto>, DepositStateRepository>()
            .AddScoped<ICRRepository<InstallmentDto>, InstallmentStateRepository>()
            .AddScoped<ICRRepository<SalaryProjectDto>, SalaryProjectStateRepository>()
            .AddScoped<ICRRepository<ClientDto>, ClientStateRepository>()
            .AddScoped<ICRRepository<CompanyDto>, CompanyStateRepository>()
            .AddScoped<ICRRepository<CompanyEmployeeDto>, CompanyEmployeeStateRepository>()
            .AddScoped<ICRRepository<BankRecordDto>, BankRecordStateRepository>()
            .AddScoped<ICRRepository<TransactionDto>, TransactionStateRepository>()
            .AddScoped<IMapper<Credit, CreditDto>, CreditMapper>()
            .AddScoped<IMapper<Deposit, DepositDto>, DepositMapper>()
            .AddScoped<IMapper<Installment, InstallmentDto>, InstallmentMapper>()
            .AddScoped<IMapper<SalaryProject, SalaryProjectDto>, SalaryProjectMapper>()
            .AddScoped<IMapper<Client, ClientDto>, ClientMapper>()
            .AddScoped<IMapper<Company, CompanyDto>, CompanyMapper>()
            .AddScoped<IMapper<CompanyEmployee, CompanyEmployeeDto>, CompanyEmployeeMapper>()
            .AddScoped<IMapper<Bank, BankDto>, BankMapper>()
            .AddScoped<IMapper<Transaction, TransactionDto>, TransactionMapper>()
            .AddScoped<IMapper<BankRecord, BankRecordDto>, BankRecordMapper>()
            .AddScoped<IMapper<User, UserDto>, UserMapper>()
            .AddScoped<IMapper<UserAction, UserActionDto>, UserActionMapper>()
            .AddScoped<IReverter<Credit>, BankServiceReverter<Credit>>()
            .AddScoped<IReverter<Deposit>, BankServiceReverter<Deposit>>()
            .AddScoped<IReverter<Installment>, BankServiceReverter<Installment>>()
            .AddScoped<IReverter<SalaryProject>, BankServiceReverter<SalaryProject>>()
            .AddScoped<IReverter<Client>, BankClientReverter<Client>>()
            .AddScoped<IReverter<Company>, BankClientReverter<Company>>()
            .AddScoped<IReverter<CompanyEmployee>, BankClientReverter<CompanyEmployee>>()
            .AddScoped<IReverter<BankRecord>, BankRecordReverter>()
            .AddScoped<IReverter<Transaction>, TransactionReverter>()
            .AddOptions<PostgresOptions>()
            .BindConfiguration("DbConnections:Postgres");
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

var app = new App(host);

await app.RunAsync();
