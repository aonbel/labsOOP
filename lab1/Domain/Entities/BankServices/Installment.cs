namespace Domain.Entities.BankServices;

public class Installment : BankService
{
    public decimal InterestRate { get; set; }
    public decimal Amount { get; set; }
}