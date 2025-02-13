namespace Domain.Entities;

public class Bank : Company
{
    public required string BankIdentificationCode { get; set; }
    public required ICollection<User> Users { get; set; }
    public required ICollection<Service> PredefinedServices { get; set; }
}