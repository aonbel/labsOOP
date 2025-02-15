namespace Domain.Entities.BankServices;

public class Credit : BankService
{
    public required decimal InterestRate { get; set; }
    public required decimal Amount { get; set; }
}