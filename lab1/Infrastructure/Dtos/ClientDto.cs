using Domain.Entities;

namespace Infrastructure.Dtos;

public class ClientDto : BankClientDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PassportSeries { get; set; }
    public int PassportNumber { get; set; }
    public string IdentificationNumber { get; set; }
    public string PhoneNumber { get; set; }
}