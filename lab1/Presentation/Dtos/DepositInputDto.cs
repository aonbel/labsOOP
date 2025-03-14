namespace Presentation.Dtos;

public class DepositInputDto
{
    public int BankId { get; set; }
    public decimal InterestRate { get; set; }
    public int TermInMonths { get; set; }
    public bool IsInteracatble { get; set; }
}