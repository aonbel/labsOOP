using Application.Interfaces;
using Domain.Dtos;
using Domain.Entities;
using Domain.Entities.BankClients;
using Domain.Entities.BankServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.Dtos;
using Presentation.Entities;
using Presentation.Interfaces;

namespace Presentation;

public class App(IHost host)
{
    private User? _currentUser;

    public async Task RunAsync()
    {
        var adminMenu = new MenuForm
        {
            Name = "Admin menu",
            Options =
            [
                ("Add bank", new UserForm<BankInputDto>
                {
                    Name = "Input bank information",
                    OnResult = async bankInputDto =>
                    {
                        var bankService = host.Services.GetService<IBankService>()!;

                        await bankService.CreateBank(new Bank
                        {
                            Address = bankInputDto.Address,
                            BankIdentificationCode = bankInputDto.BankIdentificationCode,
                            TaxIdentificationType = bankInputDto.TaxIdentificationType,
                            TaxIdentificationNumber = bankInputDto.TaxIdentificationNumber,
                            CompanyType = (CompanyType)bankInputDto.CompanyType,
                        }, CancellationToken.None);
                    }
                }),
                ("Add operator", new UserForm<UserRegistrationDto>
                {
                    Name = "Input operator login information",
                    OnResult = async loginInfoDto =>
                    {
                        var userService = host.Services.GetService<IUserService>()!;

                        await userService.CreateUserAsync(new User
                        {
                            Login = loginInfoDto.Login,
                            Password = loginInfoDto.Password,
                            Role = "Operator"
                        }, CancellationToken.None);
                    }
                }),
                ("Add manager", new UserForm<UserRegistrationDto>
                {
                    Name = "Input manager login information",
                    OnResult = async loginInfoDto =>
                    {
                        var userService = host.Services.GetService<IUserService>()!;

                        await userService.CreateUserAsync(new User
                        {
                            Login = loginInfoDto.Login,
                            Password = loginInfoDto.Password,
                            Role = "Manager"
                        }, CancellationToken.None);
                    }
                }),
                ("Get actions of user", new UserForm<BaseEntityInputDto>
                {
                    Name = "Input id of user",
                    OnResult = async baseEntityInputDto =>
                    {
                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        var userActions =
                            await userActionService.GetUserActionsByUserIdAsync(baseEntityInputDto.Id,
                                CancellationToken.None);

                        var actionsMenu = new MenuForm
                        {
                            Name = "Actions menu",
                            Options = userActions.Select(action => (action.Id.ToString(), new InfoForm<UserAction>
                            {
                                Name = $"Action {action.Id}",
                                Entity = action
                            } as IUiInstance)).ToList()
                        };

                        await actionsMenu.RunAsync();
                    }
                }),
                ("Revert action of user", new UserForm<BaseEntityInputDto>
                {
                    Name = "Input id of action",
                    OnResult = async baseEntityInputDto =>
                    {
                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        var userAction = await userActionService.GetUserActionByIdAsync(
                            baseEntityInputDto.Id,
                            CancellationToken.None);

                        var userActionTypeReversed = userAction.Type switch
                        {
                            ActionType.Create => ActionType.Delete,
                            ActionType.Update => ActionType.Update,
                            ActionType.Delete => ActionType.Create,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        await userActionService.AddUserActionAsync(
                            _currentUser!.Id,
                            await userActionService.GetCurrentStateOfActionTargetAsync(
                                userAction.Id,
                                CancellationToken.None),
                            $"Revert action {userAction.Id}",
                            userActionTypeReversed,
                            CancellationToken.None
                        );

                        await userActionService.RevertUserActionByIdAsync(
                            baseEntityInputDto.Id,
                            CancellationToken.None);
                    }
                })
            ]
        };

        var managerMenu = new MenuForm
        {
            Name = "Manager menu",
            Options =
            [
                ("Get requests for bank services", new ActionForm
                {
                    Name = "Getting data",
                    Handler = async () =>
                    {
                        var requestService = host.Services.GetService<IRequestService>()!;

                        var requests = await requestService.GetBankServiceRequestsAsync(CancellationToken.None);

                        var options = requests
                            .Select(request => ((string Label, IUiInstance))($"{request.GetType().Name} {request.Id}",
                                request switch
                                {
                                    Credit credit => new InfoForm<Credit>
                                    {
                                        Name = $"Credit {request.Id}", Entity = credit
                                    },
                                    Deposit deposit => new InfoForm<Deposit>
                                    {
                                        Name = $"Deposit {request.Id}", Entity = deposit
                                    },
                                    Installment installment => new InfoForm<Installment>
                                    {
                                        Name = $"Installment {request.Id}", Entity = installment
                                    },
                                    SalaryProject salaryProject => new InfoForm<SalaryProject>
                                    {
                                        Name = $"SalaryProject {request.Id}", Entity = salaryProject
                                    }
                                }))
                            .ToList();

                        var requestMenu = new MenuForm
                        {
                            Name = "Bank service requests",
                            Options = options
                        };

                        await requestMenu.RunAsync();
                    }
                }),
                ("Approve request for bank service", new MenuForm
                {
                    Name = "Choose service",
                    Options =
                    [
                        ("Credit", new UserForm<BaseEntityInputDto>
                        {
                            Name = "Credit request id input",
                            OnResult = async baseEntityInputDto =>
                            {
                                var requestService = host.Services.GetService<IRequestService>()!;
                                var userActionService = host.Services.GetService<IUserActionService>()!;
                                var bankServiceService = host.Services.GetService<IBankServiceService>()!;
                                var creditId = new Credit
                                {
                                    Id = baseEntityInputDto.Id,
                                };

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    await bankServiceService.GetBankServiceByIdAsync(creditId, CancellationToken.None),
                                    $"Approve request for credit {baseEntityInputDto.Id}",
                                    ActionType.Update,
                                    CancellationToken.None);

                                await requestService.ApproveBankServiceRequestAsync(creditId, CancellationToken.None);
                            }
                        }),
                        ("Deposit", new UserForm<BaseEntityInputDto>
                        {
                            Name = "Deposit request id input",
                            OnResult = async baseEntityInputDto =>
                            {
                                var requestService = host.Services.GetService<IRequestService>()!;
                                var userActionService = host.Services.GetService<IUserActionService>()!;
                                var bankServiceService = host.Services.GetService<IBankServiceService>()!;
                                var depositId = new Deposit
                                {
                                    Id = baseEntityInputDto.Id,
                                };

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    await bankServiceService.GetBankServiceByIdAsync(depositId, CancellationToken.None),
                                    $"Approve request for deposit {baseEntityInputDto.Id}",
                                    ActionType.Update,
                                    CancellationToken.None);

                                await requestService.ApproveBankServiceRequestAsync(new Deposit
                                {
                                    Id = baseEntityInputDto.Id,
                                }, CancellationToken.None);
                            }
                        }),
                        ("Installment", new UserForm<BaseEntityInputDto>
                        {
                            Name = "Installment request id input",
                            OnResult = async baseEntityInputDto =>
                            {
                                var requestService = host.Services.GetService<IRequestService>()!;
                                var userActionService = host.Services.GetService<IUserActionService>()!;
                                var bankServiceService = host.Services.GetService<IBankServiceService>()!;
                                var installmentId = new Installment
                                {
                                    Id = baseEntityInputDto.Id,
                                };

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    await bankServiceService.GetBankServiceByIdAsync(installmentId,
                                        CancellationToken.None),
                                    $"Approve request for installment {baseEntityInputDto.Id}",
                                    ActionType.Update,
                                    CancellationToken.None);

                                await requestService.ApproveBankServiceRequestAsync(new Installment
                                {
                                    Id = baseEntityInputDto.Id,
                                }, CancellationToken.None);
                            }
                        }),
                        ("SalaryProject", new UserForm<BaseEntityInputDto>
                        {
                            Name = "SalaryProject request id input",
                            OnResult = async baseEntityInputDto =>
                            {
                                var requestService = host.Services.GetService<IRequestService>()!;
                                var userActionService = host.Services.GetService<IUserActionService>()!;
                                var bankServiceService = host.Services.GetService<IBankServiceService>()!;
                                var salaryProjectId = new SalaryProject
                                {
                                    Id = baseEntityInputDto.Id,
                                };

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    await bankServiceService.GetBankServiceByIdAsync(salaryProjectId,
                                        CancellationToken.None),
                                    $"Approve request for salaryProject {baseEntityInputDto.Id}",
                                    ActionType.Update,
                                    CancellationToken.None);

                                await requestService.ApproveBankServiceRequestAsync(new SalaryProject
                                {
                                    Id = baseEntityInputDto.Id,
                                }, CancellationToken.None);
                            }
                        }),
                    ]
                }),
                ("Get requests for bank clients", new ActionForm
                {
                    Name = "Getting data",
                    Handler = async () =>
                    {
                        var requestService = host.Services.GetService<IRequestService>()!;

                        var requests = await requestService.GetBankClientRequestsAsync(CancellationToken.None);

                        var options = requests
                            .Select(request => ((string Label, IUiInstance))($"{request.GetType().Name} {request.Id}",
                                request switch
                                {
                                    CompanyEmployee companyEmployee => new InfoForm<CompanyEmployee>
                                    {
                                        Name = $"CompanyEmployee {request.Id}", Entity = companyEmployee
                                    },
                                    Company company => new InfoForm<Company>
                                    {
                                        Name = $"Company {request.Id}", Entity = company
                                    },
                                    Client client => new InfoForm<Client>
                                    {
                                        Name = $"Client {request.Id}", Entity = client
                                    },
                                    _ => throw new ArgumentOutOfRangeException(nameof(request), request, null)
                                }))
                            .ToList();

                        var requestMenu = new MenuForm
                        {
                            Name = "Bank client requests",
                            Options = options
                        };

                        await requestMenu.RunAsync();
                    }
                }),
                ("Approve request for bank client", new MenuForm
                {
                    Name = "Choose bank client type",
                    Options =
                    [
                        ("CompanyEmployee", new UserForm<BaseEntityInputDto>
                        {
                            Name = "CompanyEmployee request id input",
                            OnResult = async baseEntityInputDto =>
                            {
                                var requestService = host.Services.GetService<IRequestService>()!;
                                var userActionService = host.Services.GetService<IUserActionService>()!;
                                var bankClientService = host.Services.GetService<IBankClientService>()!;
                                
                                var companyEmployeeId = new CompanyEmployee
                                {
                                    Id = baseEntityInputDto.Id,
                                };

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    await bankClientService.GetClientByIdAsync(companyEmployeeId, CancellationToken.None),
                                    $"Approve request for client {baseEntityInputDto.Id}",
                                    ActionType.Update,
                                    CancellationToken.None);

                                await requestService.ApproveBankClientRequestAsync(companyEmployeeId, CancellationToken.None);
                            }
                        }),
                        ("Company", new UserForm<BaseEntityInputDto>
                        {
                            Name = "Company request id input",
                            OnResult = async baseEntityInputDto =>
                            {
                                var requestService = host.Services.GetService<IRequestService>()!;
                                var userActionService = host.Services.GetService<IUserActionService>()!;
                                var bankClientService = host.Services.GetService<IBankClientService>()!;
                                var companyId = new Company
                                {
                                    Id = baseEntityInputDto.Id,
                                };

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    await bankClientService.GetClientByIdAsync(companyId, CancellationToken.None),
                                    $"Approve request for company {baseEntityInputDto.Id}",
                                    ActionType.Update,
                                    CancellationToken.None);

                                await requestService.ApproveBankClientRequestAsync(companyId, CancellationToken.None);
                            }
                        }),
                        ("Client", new UserForm<BaseEntityInputDto>
                        {
                            Name = "Client request id input",
                            OnResult = async baseEntityInputDto =>
                            {
                                var requestService = host.Services.GetService<IRequestService>()!;
                                var userActionService = host.Services.GetService<IUserActionService>()!;
                                var bankClientService = host.Services.GetService<IBankClientService>()!;
                                var clientId = new Client
                                {
                                    Id = baseEntityInputDto.Id,
                                };

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    await bankClientService.GetClientByIdAsync(clientId, CancellationToken.None),
                                    $"Approve request for client {baseEntityInputDto.Id}",
                                    ActionType.Update,
                                    CancellationToken.None);

                                await requestService.ApproveBankClientRequestAsync(clientId, CancellationToken.None);
                            }
                        })
                    ]
                }),
                ("Get actions of company", new UserForm<BaseEntityInputDto>
                {
                    Name = "Input id of company",
                    OnResult = async baseEntityInputDto =>
                    {
                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        var companyActions = await userActionService.GetUserActionsByUserIdAsync(
                            baseEntityInputDto.Id,
                            CancellationToken.None);

                        var actionsMenu = new MenuForm
                        {
                            Name = "Actions menu",
                            Options = companyActions.Select(action => (action.Id.ToString(), new InfoForm<UserAction>
                            {
                                Name = $"Action {action.Id}",
                                Entity = action
                            } as IUiInstance)).ToList()
                        };

                        await actionsMenu.RunAsync();
                    }
                }),
                ("Revert action of company", new UserForm<BaseEntityInputDto>
                {
                    Name = "Input id of action",
                    OnResult = async baseEntityInputDto =>
                    {
                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        var userAction = await userActionService.GetUserActionByIdAsync(
                            baseEntityInputDto.Id,
                            CancellationToken.None);

                        var userActionTypeReversed = userAction.Type switch
                        {
                            ActionType.Create => ActionType.Delete,
                            ActionType.Update => ActionType.Update,
                            ActionType.Delete => ActionType.Create,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        await userActionService.AddUserActionAsync(
                            _currentUser!.Id,
                            await userActionService.GetCurrentStateOfActionTargetAsync(
                                userAction.Id,
                                CancellationToken.None),
                            $"Revert action {userAction.Id}",
                            userActionTypeReversed,
                            CancellationToken.None
                        );

                        await userActionService.RevertUserActionByIdAsync(
                            baseEntityInputDto.Id,
                            CancellationToken.None);
                    }
                }),
            ]
        };

        var operatorMenu = new MenuForm
        {
            Name = "Operator menu",
            Options =
            [
                ("Get actions of user", new UserForm<BaseEntityInputDto>
                {
                    Name = "Input id of user",
                    OnResult = async baseEntityInputDto =>
                    {
                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        var userActions =
                            await userActionService.GetUserActionsByUserIdAsync(baseEntityInputDto.Id,
                                CancellationToken.None);

                        var actionsMenu = new MenuForm
                        {
                            Name = "Actions menu",
                            Options = userActions.Select(action => (action.Id.ToString(), new InfoForm<UserAction>
                            {
                                Name = $"Action {action.Id}",
                                Entity = action
                            } as IUiInstance)).ToList()
                        };

                        await actionsMenu.RunAsync();
                    }
                }),
                ("Revert action of user", new UserForm<UserActionDto>
                {
                    Name = "Input id of user",
                    OnResult = async userActionDto =>
                    {
                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        var userAction = await userActionService.GetUserActionByIdAsync(
                            userActionDto.Id,
                            CancellationToken.None);

                        var userActionTypeReversed = userAction.Type switch
                        {
                            ActionType.Create => ActionType.Delete,
                            ActionType.Update => ActionType.Update,
                            ActionType.Delete => ActionType.Create,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        await userActionService.AddUserActionAsync(
                            _currentUser!.Id,
                            await userActionService.GetCurrentStateOfActionTargetAsync(
                                userAction.Id,
                                CancellationToken.None),
                            $"Revert action {userAction.Id}",
                            userActionTypeReversed,
                            CancellationToken.None
                        );

                        await userActionService.RevertUserActionByIdAsync(
                            userActionDto.UserId,
                            CancellationToken.None);
                    }
                }),
                ("Get requests for salary project", new ActionForm
                {
                    Name = "Getting data",
                    Handler = async () =>
                    {
                        var requestService = host.Services.GetService<IRequestService>()!;

                        var requests = (await requestService.GetBankServiceRequestsAsync(CancellationToken.None))
                            .Where(request => request is SalaryProject)
                            .Select(request => (request as SalaryProject)!);

                        var requestMenu = new MenuForm
                        {
                            Name = "Salary project requests",
                            Options = requests
                                .Select(request => (request.Id.ToString(), new InfoForm<SalaryProject>
                                {
                                    Name = $"Salary project {request.Id}",
                                    Entity = request
                                } as IUiInstance))
                                .ToList()
                        };

                        await requestMenu.RunAsync();
                    }
                }),
                ("Approve request for salary project", new UserForm<BaseEntityInputDto>
                {
                    Name = "Input id of user",
                    OnResult = async baseEntityInputDto =>
                    {
                        var requestService = host.Services.GetService<IRequestService>()!;
                        var bankServiceService = host.Services.GetService<IBankServiceService>()!;
                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        await userActionService.AddUserActionAsync(
                            _currentUser!.Id,
                            await bankServiceService.GetBankServiceByIdAsync(
                                new SalaryProject
                                {
                                    Id = baseEntityInputDto.Id
                                }, CancellationToken.None),
                            $"Approved request for salary project {baseEntityInputDto.Id}",
                            ActionType.Update,
                            CancellationToken.None);

                        await requestService.ApproveBankServiceRequestAsync(new SalaryProject
                        {
                            Id = baseEntityInputDto.Id,
                        }, CancellationToken.None);
                    }
                }),
            ]
        };

        var clientMenu = new MenuForm
        {
            Name = "Client menu",
            Options =
            [
                ("Records", new ActionForm
                {
                    Name = "Getting info",
                    Handler = async () =>
                    {
                        var bankRecordService = host.Services.GetService<IBankRecordService>()!;

                        var records = await bankRecordService.GetBankRecordsInfoByBankClientIdAsync(
                            _currentUser!.BankClient!,
                            CancellationToken.None);

                        var recordMenu = new MenuForm
                        {
                            Name = "Records",
                            Options = records.Select(record => (record.Id.ToString(), new InfoForm<BankRecord>
                            {
                                Name = $"Bank record {record.Id}",
                                Entity = record
                            } as IUiInstance)).ToList()
                        };

                        await recordMenu.RunAsync();
                    }
                }),
                ("Services", new ActionForm
                {
                    Name = "Getting info",
                    Handler = async () =>
                    {
                        var bankServiceService = host.Services.GetService<IBankServiceService>()!;

                        var services = await bankServiceService.GetBankServicesInfoByBankClientIdAsync(
                            _currentUser!.BankClient!,
                            CancellationToken.None);

                        var serviceMenu = new MenuForm
                        {
                            Name = "Services",
                            Options = services.Select(service => (service.Id.ToString(), new InfoForm<BankService>
                            {
                                Name = $"Bank service {service.Id}",
                                Entity = service
                            } as IUiInstance)).ToList()
                        };

                        await serviceMenu.RunAsync();
                    }
                }),
                ("Create bank record", new UserForm<BaseEntityInputDto>
                {
                    Name = "Bank record creation menu",
                    OnResult = async baseEntityInputDto =>
                    {
                        var bankRecordService = host.Services.GetService<IBankRecordService>()!;

                        var bankRecordId = await bankRecordService.CreateBankRecordAsync(_currentUser!.BankClient!,
                            baseEntityInputDto.Id,
                            CancellationToken.None);

                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        await userActionService.AddUserActionAsync(
                            _currentUser!.Id,
                            new BankRecord
                            {
                                Id = bankRecordId,
                            },
                            $"Create new bank record {bankRecordId}",
                            ActionType.Create,
                            CancellationToken.None);
                    }
                }),
                ("Create transaction", new UserForm<TransactionInputDto>
                {
                    Name = "Transaction creation menu",
                    OnResult = async transactionDto =>
                    {
                        var transactionService = host.Services.GetService<ITransactionService>()!;

                        var transactionId = await transactionService.RunTransactionAsync(
                            transactionDto.RecipientBankRecordId,
                            transactionDto.ReceiverBankRecordId, transactionDto.Amount, CancellationToken.None);

                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        await userActionService.AddUserActionAsync(
                            _currentUser!.Id,
                            new Transaction
                            {
                                Id = transactionId
                            },
                            $"Create new transaction {transactionId}",
                            ActionType.Create,
                            CancellationToken.None);
                    }
                }),
                ("Create service request", new MenuForm
                {
                    Name = "Service request creation menu",
                    Options =
                    [
                        ("Deposit", new UserForm<DepositInputDto>
                        {
                            Name = "Deposit creation menu",
                            OnResult = async depositUserDto =>
                            {
                                var bankRecordService = host.Services.GetService<IRequestService>()!;

                                var depositId = await bankRecordService.CreateBankServiceRequestAsync(
                                    depositUserDto.BankId,
                                    _currentUser!.BankClient!,
                                    new Deposit
                                    {
                                        InterestRate = depositUserDto.InterestRate,
                                        TermInMonths = depositUserDto.TermInMonths,
                                        IsInteractable = depositUserDto.IsInteracatble
                                    }, CancellationToken.None);

                                var userActionService = host.Services.GetService<IUserActionService>()!;

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    new Deposit()
                                    {
                                        Id = depositId,
                                    },
                                    $"Create new deposit request {depositId}",
                                    ActionType.Create,
                                    CancellationToken.None);
                            }
                        }),
                        ("Credit", new UserForm<CreditInputDto>
                        {
                            Name = "Credit creation menu",
                            OnResult = async creditUserDto =>
                            {
                                var bankRecordService = host.Services.GetService<IRequestService>()!;

                                var creditId = await bankRecordService.CreateBankServiceRequestAsync(
                                    creditUserDto.BankId,
                                    _currentUser!.BankClient!,
                                    new Credit
                                    {
                                        InterestRate = creditUserDto.InterestRate,
                                        TermInMonths = creditUserDto.TermInMonths,
                                        Amount = creditUserDto.Amount,
                                    }, CancellationToken.None);

                                var userActionService = host.Services.GetService<IUserActionService>()!;

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    new Credit
                                    {
                                        Id = creditId,
                                    },
                                    $"Create new credit request {creditId}",
                                    ActionType.Create,
                                    CancellationToken.None);
                            }
                        }),
                        ("Installment", new UserForm<InstallmentInputDto>
                        {
                            Name = "Installment creation menu",
                            OnResult = async installmentUserDto =>
                            {
                                var bankRecordService = host.Services.GetService<IRequestService>()!;

                                var installmentId = await bankRecordService.CreateBankServiceRequestAsync(
                                    installmentUserDto.BankId,
                                    _currentUser!.BankClient!,
                                    new Installment
                                    {
                                        InterestRate = installmentUserDto.InterestRate,
                                        TermInMonths = installmentUserDto.TermInMonths,
                                        Amount = installmentUserDto.Amount
                                    }, CancellationToken.None);

                                var userActionService = host.Services.GetService<IUserActionService>()!;

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    new Installment
                                    {
                                        Id = installmentId
                                    },
                                    $"Create new installment request {installmentId}",
                                    ActionType.Create,
                                    CancellationToken.None);
                            }
                        }),
                    ]
                }),
                ("Deposit", new UserForm<DepositAmountInputDto>
                {
                    Name = "Deposit menu",
                    OnResult = async depositAmountUserDto =>
                    {
                        var bankRecordService = host.Services.GetService<IBankRecordService>()!;
                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        await userActionService.AddUserActionAsync(
                            _currentUser!.Id,
                            await bankRecordService.GetBankRecordByIdAsync(
                                depositAmountUserDto.BankRecordId,
                                CancellationToken.None),
                            $"Deposit {depositAmountUserDto.Amount} to bank record {depositAmountUserDto.BankRecordId}",
                            ActionType.Update,
                            CancellationToken.None);

                        await bankRecordService.DepositAmountFromBankRecordByIdAsync(
                            depositAmountUserDto.BankRecordId,
                            depositAmountUserDto.Amount,
                            CancellationToken.None);
                    }
                }),
                ("Withdraw", new UserForm<WithdrawAmountInputDto>
                {
                    Name = "Withdraw menu",
                    OnResult = async withdrawAmountUserDto =>
                    {
                        var bankRecordService = host.Services.GetService<IBankRecordService>()!;
                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        await userActionService.AddUserActionAsync(
                            _currentUser!.Id,
                            await bankRecordService.GetBankRecordByIdAsync(
                                withdrawAmountUserDto.BankRecordId,
                                CancellationToken.None),
                            $"Withdraw {withdrawAmountUserDto.Amount} to bank record {withdrawAmountUserDto.BankRecordId}",
                            ActionType.Update,
                            CancellationToken.None);

                        await bankRecordService.WithdrawAmountFromBankRecordByIdAsync(
                            withdrawAmountUserDto.BankRecordId,
                            withdrawAmountUserDto.Amount,
                            CancellationToken.None);
                    }
                })
            ]
        };

        var companyMenu = new MenuForm
        {
            Name = "Company menu",
            Options =
            [
                ("Records", new ActionForm
                {
                    Name = "Getting info",
                    Handler = async () =>
                    {
                        var bankRecordService = host.Services.GetService<IBankRecordService>()!;

                        var records = await bankRecordService.GetBankRecordsInfoByBankClientIdAsync(
                            _currentUser!.BankClient!,
                            CancellationToken.None);

                        var recordMenu = new MenuForm
                        {
                            Name = "Records",
                            Options = records.Select(record => (record.Id.ToString(), new InfoForm<BankRecord>
                            {
                                Name = $"Bank record {record.Id}",
                                Entity = record
                            } as IUiInstance)).ToList()
                        };

                        await recordMenu.RunAsync();
                    }
                }),
                ("Services", new ActionForm
                {
                    Name = "Getting info",
                    Handler = async () =>
                    {
                        var bankServiceService = host.Services.GetService<IBankServiceService>()!;

                        var services = await bankServiceService.GetBankServicesInfoByBankClientIdAsync(
                            _currentUser!.BankClient!,
                            CancellationToken.None);

                        var serviceMenu = new MenuForm
                        {
                            Name = "Services",
                            Options = services.Select(service => (service.Id.ToString(), new InfoForm<BankService>
                            {
                                Name = $"Bank service {service.Id}",
                                Entity = service
                            } as IUiInstance)).ToList()
                        };

                        await serviceMenu.RunAsync();
                    }
                }),
                ("Create bank record", new UserForm<BaseEntityInputDto>
                {
                    Name = "Bank record creation menu",
                    OnResult = async baseEntityInputDto =>
                    {
                        var bankRecordService = host.Services.GetService<IBankRecordService>()!;

                        var bankRecordId = await bankRecordService.CreateBankRecordAsync(_currentUser!.BankClient!,
                            baseEntityInputDto.Id,
                            CancellationToken.None);

                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        await userActionService.AddUserActionAsync(
                            _currentUser!.Id,
                            new BankRecord
                            {
                                Id = bankRecordId,
                            },
                            $"Create new bank record {bankRecordId}",
                            ActionType.Create,
                            CancellationToken.None);
                    }
                }),
                ("Create transaction", new UserForm<TransactionInputDto>
                {
                    Name = "Transaction creation menu",
                    OnResult = async transactionDto =>
                    {
                        var transactionService = host.Services.GetService<ITransactionService>()!;

                        var transactionId = await transactionService.RunTransactionAsync(
                            transactionDto.RecipientBankRecordId,
                            transactionDto.ReceiverBankRecordId, transactionDto.Amount, CancellationToken.None);

                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        await userActionService.AddUserActionAsync(
                            _currentUser!.Id,
                            new Transaction
                            {
                                Id = transactionId
                            },
                            $"Create new transaction {transactionId}",
                            ActionType.Create,
                            CancellationToken.None);
                    }
                }),
                ("Create service request", new MenuForm
                {
                    Name = "Service request creation menu",
                    Options =
                    [
                        ("Deposit", new UserForm<DepositInputDto>
                        {
                            Name = "Deposit creation menu",
                            OnResult = async depositUserDto =>
                            {
                                var bankRecordService = host.Services.GetService<IRequestService>()!;

                                var depositId = await bankRecordService.CreateBankServiceRequestAsync(
                                    depositUserDto.BankId,
                                    _currentUser!.BankClient!,
                                    new Deposit
                                    {
                                        InterestRate = depositUserDto.InterestRate,
                                        TermInMonths = depositUserDto.TermInMonths,
                                        IsInteractable = depositUserDto.IsInteracatble
                                    }, CancellationToken.None);

                                var userActionService = host.Services.GetService<IUserActionService>()!;

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    new Deposit()
                                    {
                                        Id = depositId,
                                    },
                                    $"Create new deposit request {depositId}",
                                    ActionType.Create,
                                    CancellationToken.None);
                            }
                        }),
                        ("Credit", new UserForm<CreditInputDto>
                        {
                            Name = "Credit creation menu",
                            OnResult = async creditUserDto =>
                            {
                                var bankRecordService = host.Services.GetService<IRequestService>()!;

                                var creditId = await bankRecordService.CreateBankServiceRequestAsync(
                                    creditUserDto.BankId,
                                    _currentUser!.BankClient!,
                                    new Credit
                                    {
                                        InterestRate = creditUserDto.InterestRate,
                                        TermInMonths = creditUserDto.TermInMonths,
                                        Amount = creditUserDto.Amount,
                                    }, CancellationToken.None);

                                var userActionService = host.Services.GetService<IUserActionService>()!;

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    new Credit
                                    {
                                        Id = creditId,
                                    },
                                    $"Create new credit request {creditId}",
                                    ActionType.Create,
                                    CancellationToken.None);
                            }
                        }),
                        ("Installment", new UserForm<InstallmentInputDto>
                        {
                            Name = "Installment creation menu",
                            OnResult = async installmentUserDto =>
                            {
                                var bankRecordService = host.Services.GetService<IRequestService>()!;

                                var installmentId = await bankRecordService.CreateBankServiceRequestAsync(
                                    installmentUserDto.BankId,
                                    _currentUser!.BankClient!,
                                    new Installment
                                    {
                                        InterestRate = installmentUserDto.InterestRate,
                                        TermInMonths = installmentUserDto.TermInMonths,
                                        Amount = installmentUserDto.Amount
                                    }, CancellationToken.None);

                                var userActionService = host.Services.GetService<IUserActionService>()!;

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    new Installment
                                    {
                                        Id = installmentId
                                    },
                                    $"Create new installment request {installmentId}",
                                    ActionType.Create,
                                    CancellationToken.None);
                            }
                        }),
                        ("SalaryProject", new UserForm<SalaryProjectInputDto>
                        {
                            Name = "SalaryProject creation menu",
                            OnResult = async salaryProjectInputDto =>
                            {
                                var bankRecordService = host.Services.GetService<IRequestService>()!;

                                var salaryProjectId = await bankRecordService.CreateBankServiceRequestAsync(
                                    salaryProjectInputDto.BankId,
                                    _currentUser!.BankClient!,
                                    new SalaryProject
                                    {
                                        TermInMonths = salaryProjectInputDto.TermInMonths,
                                        Company = new Company
                                        {
                                            Id = _currentUser!.BankClient!.Id
                                        }
                                    }, CancellationToken.None);

                                var userActionService = host.Services.GetService<IUserActionService>()!;

                                await userActionService.AddUserActionAsync(
                                    _currentUser!.Id,
                                    new Deposit()
                                    {
                                        Id = salaryProjectId,
                                    },
                                    $"Create new request for salaryProject {salaryProjectId}",
                                    ActionType.Create,
                                    CancellationToken.None);
                            }
                        })
                    ]
                }),
                ("Deposit", new UserForm<DepositAmountInputDto>
                {
                    Name = "Deposit menu",
                    OnResult = async depositAmountUserDto =>
                    {
                        var bankRecordService = host.Services.GetService<IBankRecordService>()!;
                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        await userActionService.AddUserActionAsync(
                            _currentUser!.Id,
                            await bankRecordService.GetBankRecordByIdAsync(
                                depositAmountUserDto.BankRecordId,
                                CancellationToken.None),
                            $"Deposit {depositAmountUserDto.Amount} to bank record {depositAmountUserDto.BankRecordId}",
                            ActionType.Update,
                            CancellationToken.None);

                        await bankRecordService.DepositAmountFromBankRecordByIdAsync(
                            depositAmountUserDto.BankRecordId,
                            depositAmountUserDto.Amount,
                            CancellationToken.None);
                    }
                }),
                ("Withdraw", new UserForm<WithdrawAmountInputDto>
                {
                    Name = "Withdraw menu",
                    OnResult = async withdrawAmountUserDto =>
                    {
                        var bankRecordService = host.Services.GetService<IBankRecordService>()!;
                        var userActionService = host.Services.GetService<IUserActionService>()!;

                        await userActionService.AddUserActionAsync(
                            _currentUser!.Id,
                            await bankRecordService.GetBankRecordByIdAsync(
                                withdrawAmountUserDto.BankRecordId,
                                CancellationToken.None),
                            $"Withdraw {withdrawAmountUserDto.Amount} to bank record {withdrawAmountUserDto.BankRecordId}",
                            ActionType.Update,
                            CancellationToken.None);

                        await bankRecordService.WithdrawAmountFromBankRecordByIdAsync(
                            withdrawAmountUserDto.BankRecordId,
                            withdrawAmountUserDto.Amount,
                            CancellationToken.None);
                    }
                })
            ]
        };

        var loginForm = new UserForm<LoginDto>
        {
            Name = "Login form",
            OnResult = async userDto =>
            {
                var userHandler = host.Services.GetService<IUserService>()!;

                try
                {
                    _currentUser = await userHandler.GetUserByLoginAsync(userDto.Login, CancellationToken.None);
                }
                catch (Exception e)
                {
                    return;
                }

                if (_currentUser.Password != userDto.Password)
                {
                    return;
                }

                switch (_currentUser.Role)
                {
                    case "Client":
                    case "CompanyEmployee":
                        if (_currentUser!.BankClient!.IsApproved)
                        {
                            await clientMenu.RunAsync();
                        }

                        break;
                    case "Company":
                        if (_currentUser!.BankClient!.IsApproved)
                        {
                            await companyMenu.RunAsync();
                        }

                        break;
                    case "Operator":
                        await operatorMenu.RunAsync();
                        break;
                    case "Manager":
                        await managerMenu.RunAsync();
                        break;
                    case "Admin":
                        await adminMenu.RunAsync();
                        break;
                }
            }
        };

        var registrationAsCompanyForm = new UserForm<CompanyRegistrationDto>
        {
            Name = "Registration form",
            OnResult = async registrationDto =>
            {
                var company = new Company
                {
                    Address = registrationDto.Address,
                    CompanyType = (CompanyType)registrationDto.CompanyType,
                    TaxIdentificationNumber = registrationDto.TaxIdentificationNumber,
                    TaxIdentificationType = registrationDto.TaxIdentificationType
                };

                var requestService = host.Services.GetService<IRequestService>()!;
                company.Id = await requestService.CreateBankClientRequestAsync(company, CancellationToken.None);

                var newUser = new User
                {
                    Login = registrationDto.Login,
                    Password = registrationDto.Password,
                    Role = "Company",
                    BankClient = company
                };

                var userHandler = host.Services.GetService<IUserService>()!;
                await userHandler.CreateUserAsync(newUser, CancellationToken.None);
            }
        };

        var registrationAsClientForm = new UserForm<ClientRegistrationDto>
        {
            Name = "Registration form",
            OnResult = async registrationDto =>
            {
                var client = new Client
                {
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName,
                    Email = registrationDto.Email,
                    PassportNumber = registrationDto.PassportNumber,
                    PassportSeries = registrationDto.PassportSeries,
                    IdentificationNumber = registrationDto.IdentificationNumber,
                    PhoneNumber = registrationDto.PhoneNumber
                };

                var requestService = host.Services.GetService<IRequestService>()!;
                client.Id = await requestService.CreateBankClientRequestAsync(client, CancellationToken.None);

                var newUser = new User
                {
                    Login = registrationDto.Login,
                    Password = registrationDto.Password,
                    Role = "Client",
                    BankClient = client
                };

                var userHandler = host.Services.GetService<IUserService>()!;
                await userHandler.CreateUserAsync(newUser, CancellationToken.None);
            }
        };

        var registrationAsCompanyEmployee = new UserForm<CompanyEmployeeRegistrationDto>
        {
            Name = "Registration form",
            OnResult = async registrationDto =>
            {
                var companyEmployee = new CompanyEmployee
                {
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName,
                    Email = registrationDto.Email,
                    PassportNumber = registrationDto.PassportNumber,
                    PassportSeries = registrationDto.PassportSeries,
                    IdentificationNumber = registrationDto.IdentificationNumber,
                    PhoneNumber = registrationDto.PhoneNumber,
                    SalaryProject = new SalaryProject
                    {
                        Id = registrationDto.SalaryProjectId,
                    },
                    Salary = registrationDto.Salary,
                    Position = registrationDto.Position
                };

                var requestService = host.Services.GetService<IRequestService>()!;
                companyEmployee.Id = await requestService.CreateBankClientRequestAsync(
                    companyEmployee,
                    CancellationToken.None);

                var newUser = new User
                {
                    Login = registrationDto.Login,
                    Password = registrationDto.Password,
                    Role = "CompanyEmployee",
                    BankClient = companyEmployee
                };

                var userHandler = host.Services.GetService<IUserService>()!;
                await userHandler.CreateUserAsync(newUser, CancellationToken.None);
            }
        };

        var bankMenu = new ActionForm
        {
            Name = "Getting info",
            Handler = async () =>
            {
                var bankService = host.Services.GetService<IBankService>()!;

                var banksInfo = await bankService.GetBanksInfoAsync(CancellationToken.None);

                var options = banksInfo
                    .Select(bank => (bank.Id.ToString(), new InfoForm<Bank>
                    {
                        Name = "Bank info",
                        Entity = bank,
                    } as IUiInstance))
                    .ToList();

                var bankMenu = new MenuForm
                {
                    Name = "Bank menu",
                    Options = options,
                };

                await bankMenu.RunAsync();
            }
        };

        var mainMenu = new MenuForm
        {
            Name = "Authorization",
            Options =
            [
                ("Get list of banks", bankMenu),
                ("Login", loginForm),
                ("Register as a client", registrationAsClientForm),
                ("Register as a company", registrationAsCompanyForm),
                ("Register as a company employee", registrationAsCompanyEmployee)
            ]
        };

        await mainMenu.RunAsync();
    }
}