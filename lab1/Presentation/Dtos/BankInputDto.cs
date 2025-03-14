namespace Presentation.Dtos;

public class BankInputDto
{
    public int CompanyType { get; set; }
    
    public string TaxIdentificationNumber { get; set; }
    
    public string TaxIdentificationType { get; set; }
    
    public string Address { get; set; }
    
    public string BankIdentificationCode { get; set; } 
}