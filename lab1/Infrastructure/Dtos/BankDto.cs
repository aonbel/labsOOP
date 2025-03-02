namespace Infrastructure.Dtos;

public class BankDto : CompanyDto
{
    public string BankIdentificationCode { get; set; }
    public ICollection<int> ClientIds { get; set; }
}