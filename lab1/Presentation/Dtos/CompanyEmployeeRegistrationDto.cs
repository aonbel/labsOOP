namespace Presentation.Dtos;

public class CompanyEmployeeRegistrationDto
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PassportSeries { get; set; }
    public int PassportNumber { get; set; }
    public string IdentificationNumber { get; set; }
    public string PhoneNumber { get; set; }
    public int SalaryProjectId { get; set; }
    public string Position { get; set; }
    public decimal Salary { get; set; }
}