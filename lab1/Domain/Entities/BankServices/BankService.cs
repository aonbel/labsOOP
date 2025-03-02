using Domain.Entities.Core;

namespace Domain.Entities.BankServices;

public class BankService : Entity
{
    public DateTime LastUpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ClosedAt { get; set; }
    public int TermInMonths  { get; set; }
    
    public BankRecord Record { get; set; }
    public bool IsApproved { get; set; }
}