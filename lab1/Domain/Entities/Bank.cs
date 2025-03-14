using Domain.Entities.BankClients;

namespace Domain.Entities;

public class Bank : Company
{
    public string BankIdentificationCode { get; set; }
    public ICollection<Client> Users { get; set; }
}