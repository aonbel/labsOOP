using Domain.Interfaces;

namespace Domain.Entities;

public class Transaction : Entity
{
    public required ITransactionMember Recipient { get; set; }
    public required ITransactionMember Receiver { get; set; }
    public required DateTime Date { get; set; }
    
    public required decimal Amount { get; set; }
}