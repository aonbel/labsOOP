using Domain.Entities.Core;

namespace Domain.Entities;

public class BankRecord : Entity
{
    public decimal Amount { get; set; }
    public bool IsActive { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
}