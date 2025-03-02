using Domain.Entities.BankClients;
using Domain.Entities.Core;

namespace Domain.Entities.Users;

public class Client : Entity
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public BankClient? BankClient { get; set; }
}