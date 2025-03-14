namespace Presentation.Dtos;

public class CompanyRegistrationDto 
{
    public string Login { get; set; }
    public string Password { get; set; }
    public int CompanyType { get; set; }
    public string TaxIdentificationNumber { get; set; }
    public string TaxIdentificationType { get; set; }
    public string Address { get; set; }
}