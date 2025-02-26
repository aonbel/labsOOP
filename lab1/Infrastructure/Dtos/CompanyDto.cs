using Domain.Entities.BankClients;

namespace Infrastructure.Dtos;

public class CompanyDto
{
    public int Id { get; set; }
    public int BankId { get; set; }
    
    public CompanyType CompanyType { get; set; }
    public string TaxIdentificationNumber { get; set; }
    public string TaxIdentificationType { get; set; }
    public string Address { get; set; }
    
    public ICollection<int> ServiceIds { get; set; }
    public ICollection<int> RecordIds { get; set; }
}