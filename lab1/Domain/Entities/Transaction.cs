using Domain.Entities.Core;

namespace Domain.Entities;

public class Transaction : BaseEntity
{
    public BankRecord RecipientBankRecord { get; set; }
    public BankRecord ReceiverBankRecord { get; set; }
    
    public bool IsCancelled { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
}