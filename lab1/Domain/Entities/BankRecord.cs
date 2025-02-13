namespace Domain.Entities;

public class BankRecord : Entity
{
    public required decimal Amount { get; set; }
    public bool IsActive { get; set; }
    public required ICollection<Transaction> Transactions { get; set; }
}