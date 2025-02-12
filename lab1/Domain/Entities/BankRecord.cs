using Domain.Interfaces;

namespace Domain.Entities;

public class BankRecord : Entity, ITransactionMember
{
    public required decimal Amount { get; set; }
}