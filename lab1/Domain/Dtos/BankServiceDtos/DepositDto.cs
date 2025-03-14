namespace Domain.Dtos.BankServiceDtos;

public class DepositDto : BankServiceDto
{
    public decimal InterestRate { get; set; }
    public bool IsInteractable { get; set; }
}