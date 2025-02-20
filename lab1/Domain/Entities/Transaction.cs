using System.Dynamic;
using Domain.Entities.Core;

namespace Domain.Entities;

public class Transaction : Entity
{
    public BankRecord RecipientBankRecord { get; set; }
    public BankRecord ReceiverBankRecord { get; set; }
    
    public bool IsCancelled { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }

    public Transaction(
        BankRecord recipientBankRecord, 
        BankRecord receiverBankRecord, 
        decimal amount, 
        DateTime date, 
        string name = "")
    {
        RecipientBankRecord = recipientBankRecord;
        ReceiverBankRecord = receiverBankRecord;
        Amount = amount;
        Date = date;
        Name = name;
        IsCancelled = amount < 0;
    }
}