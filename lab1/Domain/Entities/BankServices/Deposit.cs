namespace Domain.Entities.BankServices;

public class Deposit : BankService
{
    public decimal InterestRate { get; set; }
    public bool IsInteractable { get; set; }
}