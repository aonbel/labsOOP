namespace Infrastructure.Dtos;

public class TransactionDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int RecipientBankRecordId { get; set; }
    public int ReceiverBankRecordId { get; set; }

    public bool IsCancelled { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
}