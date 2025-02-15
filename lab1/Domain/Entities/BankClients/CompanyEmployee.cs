using Domain.Entities.BankServices;

namespace Domain.Entities.BankClients;

public class CompanyEmployee : Client
{
    public required string Position { get; set; }
    public required decimal Salary { get; set; }
    public required SalaryProject SalaryProject { get; set; }
}