namespace Domain.Entities;

public class Service : Entity
{
    public required DateTime LastUpdatedAt { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime ClosedAt { get; set; }
    public required int TermInMonths  { get; set; }
    
    public required BankRecord Record { get; set; }
}