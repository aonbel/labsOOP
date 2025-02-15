using Domain.Entities.Core;

namespace Domain.Entities;

public class BankRecord : Entity
{
    public required decimal Amount { get; set; }
    public bool IsActive { get; set; }
    public required ICollection<Transaction> Transactions { get; set; }
    public required Bank Bank { get; set; }
}