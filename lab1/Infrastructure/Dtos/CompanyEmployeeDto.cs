namespace Infrastructure.Dtos;

public class CompanyEmployeeDto : ClientDto
{
    public string Position { get; set; }
    public decimal Salary { get; set; }
    public int SalaryProjectId { get; set; }
}