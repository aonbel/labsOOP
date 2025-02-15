using Domain.Entities.BankServices;
using Domain.Entities.Core;

namespace Domain.Entities.BankClients;

public class BankClient : BaseEntity
{
    public required ICollection<BankService> Services { get; set; }
    public required ICollection<BankRecord> Records { get; set; }
}