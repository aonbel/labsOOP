namespace Domain.Entities;

public class Deposit : Service
{
    public required decimal InterestRate { get; set; }
    public required bool IsInteractable { get; set; }
}