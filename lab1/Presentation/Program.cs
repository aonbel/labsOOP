using Application.Handler;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.BankServices;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Mappers;
using Infrastructure.Options;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Dtos;
using Presentation.Entities;

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
            .AddScoped<IMapper<Credit, CreditDto>, CreditMapper>()
            .AddScoped<IMapper<Deposit, DepositDto>, DepositMapper>()
            .AddScoped<IMapper<Installment, InstallmentDto>, InstallmentMapper>()
            .AddScoped<IMapper<SalaryProject, SalaryProjectDto>, SalaryProjectMapper>()
            .AddOptions<PostgresOptions>()
            .BindConfiguration("DbConnections:Postgres");
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

var bankHandler = host.Services.GetService<IBankHandler>()!;
var userHandler = host.Services.GetService<IUserHandler>()!;

var loginForm = new UserForm<LoginDto>
{
    Name = "Login form",
    OnResult = async userDto =>
    {
        User currentUser;

        currentUser = await userHandler.GetUserByLoginAsync(userDto.Login, CancellationToken.None);

        if (currentUser.Password != userDto.Password)
        {
            return;
        }
        
        var bankClientMenu = new Menu
        {
            Name = "Bank client menu",
            Options =
            [
                ("Records", new Menu()),
                ("Services", new Menu())
            ]
        };

        await bankClientMenu.RunAsync();
    }
};

var registerForm = new UserForm<RegisterDto>
{
    Name = "Register form",
    OnResult = async RegistrationDto =>
    {
        var currentUser = new User
        {
            Login = RegistrationDto.Login,
            Password = RegistrationDto.Password,
            Role = "BankClient"
        };
        
        await userHandler.CreateUserAsync(currentUser, CancellationToken.None);
    }
};

var menu = new Menu
{
    Name = "Authorization",
    Options =
    [
        ("Login", loginForm),
        ("Register", registerForm)
    ]
};

await menu.RunAsync();