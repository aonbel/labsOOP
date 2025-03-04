using Domain.Entities.BankClients;

namespace Domain.Entities.BankServices;

public class SalaryProject : BankService
{
    public Company Company { get; set; }
}