namespace Presentation.Dtos;

public class TransactionInputDto
{
    public int RecipientBankRecordId { get; set; }
    public int ReceiverBankRecordId { get; set; }
    public decimal Amount { get; set; }
}