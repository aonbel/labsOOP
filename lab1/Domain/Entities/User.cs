namespace Domain.Entities;

public class User : BankUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PassportSeries { get; set; }
    public required int PassportNumber { get; set; }
    public required string IdentificationNumber { get; set; }
    public required string PhoneNumber { get; set; }
}