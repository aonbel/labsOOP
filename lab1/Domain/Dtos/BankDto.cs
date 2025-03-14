using Domain.Dtos.BankClientDtos;

namespace Domain.Dtos;

public class BankDto : CompanyDto
{
    public string BankIdentificationCode { get; set; }
}