namespace Infrastructure.Dtos;

public class CreditDto : BankServiceDto
{
    public decimal InterestRate { get; set; }
    public decimal Amount { get; set; }
}