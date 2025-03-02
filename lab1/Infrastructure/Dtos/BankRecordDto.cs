namespace Infrastructure.Dtos;

public class BankRecordDto : BaseEntityDto
{
    public int? CompanyEmployeeId { get; set; }
    public int? CompanyId { get; set; }
    public int? ClientId { get; set; }
    
    public int BankId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsActive { get; set; } = true;
}