using Domain.Entities.BankServices;
using Domain.Entities.Core;

namespace Domain.Entities.BankClients;

public class BankClient : BaseEntity
{
    public ICollection<BankService> Services { get; set; }
    public ICollection<BankRecord> Records { get; set; }
    
    public bool IsApproved { get; set; }
}