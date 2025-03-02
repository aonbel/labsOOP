namespace Infrastructure.Dtos;

public class SalaryProjectDto : BankServiceDto
{
    public ICollection<int> EmployeeIds { get; set; }
}