namespace Infrastructure.Dtos;

public class DepositDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal InterestRate { get; set; }
    public bool IsInteractable { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ClosedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public int TermInMonths { get; set; }
    public bool isApproved { get; set; }
    public int BankRecordId { get; set; }
}