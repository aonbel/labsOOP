namespace Domain.Dtos.BankClientDtos;

public class CompanyEmployeeDto : ClientDto
{
    public string Position { get; set; }
    public decimal Salary { get; set; }
    public int SalaryProjectId { get; set; }
}