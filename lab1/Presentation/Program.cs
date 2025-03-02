using Application.Handler;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services
            .AddScoped<IBankClientHandler, BankClientHandler>()
            .AddScoped<IBankHandler, BankHandler>()
            .AddScoped<IBankRecordHandler, BankRecordHandler>()
            .AddScoped<IBankServiceHandler, BankServiceHandler>()
            .AddScoped<ITransactionHandler, TransactionHandler>()
            .AddScoped<IUserHandler, UserHandler>()
            .AddScoped<IBankRecordRepository, BankRecordRepository>()
            .AddScoped<ICompanyEmployeeRepository, CompanyEmployeeRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ITransactionRepository, TransactionRepository>()
            .AddScoped<ICreditRepository, CreditRepository>()
            .AddScoped<IDepositRepository, DepositRepository>()
            .AddScoped<IInstallmentRepository, InstallmentRepository>()
            .AddScoped<ISalaryProjectRepository, SalaryProjectRepository>()
            .AddScoped<IRepository<BankDto>, BankRepository>()
            .AddScoped<IRepository<ClientDto>, ClientRepository>()
            .AddScoped<IRepository<CompanyDto>, CompanyRepository>()
            .AddOptions<PostgresOptions>()
            .BindConfiguration("DbConnections:Postgres");
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

var bankClientHandler = host.Services.GetService<IBankClientHandler>()!;
var bankHandler = host.Services.GetService<IBankHandler>()!;

var bankId = await bankHandler.CreateBank(new Bank
{
    Address = "GIKALO 9",
    TaxIdentificationNumber = "12345678",
    TaxIdentificationType = "asd",
    CompanyType = CompanyType.SoleProprietorship,
    BankIdentificationCode = "52"
}, CancellationToken.None);

var clientId = await bankClientHandler.CreateClientAsync(new Client
{
    FirstName = "Evgeny",
    LastName = "Prigozhin",
    Email = "Evgeny.Prigozhin@gmail.com",
    IdentificationNumber = "",
    PassportNumber = 12345678,
    PassportSeries = "ASD",
    PhoneNumber = "+375445894469"
}, CancellationToken.None);

var bankServiceHandler = host.Services.GetService<IBankServiceHandler>()!;

var serviceId = await bankServiceHandler.CreateBankService(new Client
{
    Id = clientId
}, new Credit
{
    Amount = 1000000000,
    InterestRate = 100,
    Name = "lox"
}, new Bank
{
    Id = bankId
}, CancellationToken.None);