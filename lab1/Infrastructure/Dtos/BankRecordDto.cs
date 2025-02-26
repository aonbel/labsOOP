namespace Infrastructure.Dtos;

public class BankRecordDto
{
    public int Id { get; set; }
    public int BankClientId { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public bool IsActive { get; set; }
}