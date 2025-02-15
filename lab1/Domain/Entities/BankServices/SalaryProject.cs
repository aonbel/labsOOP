using Domain.Entities.BankClients;

namespace Domain.Entities.BankServices;

public class SalaryProject : BankService
{
    public required Company Company { get; set; }
    public required ICollection<CompanyEmployee> Employees { get; set; }
}