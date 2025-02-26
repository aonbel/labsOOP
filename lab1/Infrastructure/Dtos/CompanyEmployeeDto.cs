namespace Infrastructure.Dtos;

public class CompanyEmployeeDto
{
    public int Id { get; set; }
    public int BankId { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PassportSeries { get; set; }
    public int PassportNumber { get; set; }
    public string IdentificationNumber { get; set; }
    public string PhoneNumber { get; set; }
    
    public string Position { get; set; }
    public decimal Salary { get; set; }
    public int SalaryProjectId { get; set; }
    
    public ICollection<int> ServiceIds { get; set; }
    public ICollection<int> RecordIds { get; set; }
}