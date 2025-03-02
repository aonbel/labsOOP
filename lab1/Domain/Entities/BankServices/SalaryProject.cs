using Domain.Entities.BankClients;

namespace Domain.Entities.BankServices;

public class SalaryProject : BankService
{
    public ICollection<CompanyEmployee> Employees { get; set; }
}