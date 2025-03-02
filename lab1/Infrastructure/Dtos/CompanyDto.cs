using Domain.Entities.BankClients;

namespace Infrastructure.Dtos;

public class CompanyDto : BankClientDto
{
    public CompanyType CompanyType { get; set; }
    public string TaxIdentificationNumber { get; set; }
    public string TaxIdentificationType { get; set; }
    public string Address { get; set; }
}