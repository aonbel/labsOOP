using Domain.Entities.BankClients;
using Domain.Entities.Core;

namespace Domain.Entities;

public class BankRecord : BaseEntity
{
    public decimal Amount { get; set; }
    public bool IsActive { get; set; }
    public Bank Bank { get; set; }
    public BankClient BankClient { get; set; }
}