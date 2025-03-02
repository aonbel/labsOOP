namespace Infrastructure.Dtos;

public class InstallmentDto : BankServiceDto
{
    public decimal InterestRate { get; set; }
    public decimal Amount { get; set; }
}