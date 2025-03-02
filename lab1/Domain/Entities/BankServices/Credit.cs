namespace Domain.Entities.BankServices;

public class Credit : BankService
{
    public decimal InterestRate { get; set; }
    public decimal Amount { get; set; }
}