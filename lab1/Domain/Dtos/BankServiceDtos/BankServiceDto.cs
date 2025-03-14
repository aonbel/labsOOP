namespace Domain.Dtos.BankServiceDtos;

public class BankServiceDto : BaseEntityDto
{
    public DateTime CreatedAt { get; set; }
    
    public DateTime ClosedAt { get; set; }
    
    public DateTime LastUpdatedAt { get; set; }
    
    public int TermInMonths { get; set; }
    
    public int BankRecordId { get; set; }
    
    public bool IsApproved { get; set; }
}