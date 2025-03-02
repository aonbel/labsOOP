using Domain.Entities.BankServices;

namespace Domain.Entities.BankClients;

public class CompanyEmployee : Client
{
    public string Position { get; set; }
    public decimal Salary { get; set; }
}