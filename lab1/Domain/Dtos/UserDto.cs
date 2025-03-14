namespace Domain.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public int? CompanyEmployeeId { get; set; }
    public int? CompanyId { get; set; }
    public int? ClientId { get; set; }
}