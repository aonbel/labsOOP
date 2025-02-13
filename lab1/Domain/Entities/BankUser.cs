namespace Domain.Entities;

public class BankUser
{
    public required int Id { get; set; }
    
    public required ICollection<Service> Services { get; set; }
    public required ICollection<BankRecord> Records { get; set; }
}