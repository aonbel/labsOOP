using Domain.Entities.BankClients;
using Domain.Entities.BankServices.Predefined;

namespace Domain.Entities;

public class Bank : Company
{
    public required string BankIdentificationCode { get; set; }
    public required ICollection<Client> Users { get; set; }
    public required ICollection<PredefinedBankService> PredefinedServices { get; set; }
}