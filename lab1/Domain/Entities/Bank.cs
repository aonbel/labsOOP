using Domain.Entities.BankClients;
using Domain.Entities.BankServices.Predefined;

namespace Domain.Entities;

public class Bank : Company
{
    public string BankIdentificationCode { get; set; }
    public ICollection<Client> Users { get; set; }
    public ICollection<PredefinedBankService> PredefinedServices { get; set; }
}