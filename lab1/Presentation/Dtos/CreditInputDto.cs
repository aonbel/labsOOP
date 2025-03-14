namespace Presentation.Dtos;

public class CreditInputDto
{
    public int BankId { get; set; }
    public decimal InterestRate { get; set; }
    public int TermInMonths { get; set; }
    public decimal Amount { get; set; }
}