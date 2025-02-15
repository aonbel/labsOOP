namespace Domain.Entities.BankServices;

public class Deposit : BankService
{
    public required decimal InterestRate { get; set; }
    public required bool IsInteractable { get; set; }
}