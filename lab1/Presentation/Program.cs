using Application.Handler;
using Application.Interfaces;
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
            .AddScoped<IBankRecordHandler, BankRecordHandler>()
            .AddScoped<IRepository<BankRecordDto>, BankRecordRepository>()
            .AddOptions<PostgresOptions>()
            .BindConfiguration("DbConnections:Postgres");
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

var bankRecordHandler = host.Services.GetService<IBankRecordHandler>();

await bankRecordHandler.CreateBankRecordAsync(1, new BankRecordDto {Amount = 100, IsActive = true, Name = "Naming"}, CancellationToken.None);
