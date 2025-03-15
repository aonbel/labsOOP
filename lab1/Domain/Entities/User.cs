using Domain.Entities.BankClients;
using Domain.Entities.Core;

namespace Domain.Entities;

public class User : Entity
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public BankClient? BankClient { get; set; }
}