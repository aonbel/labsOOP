using Domain.Interfaces;

namespace Domain.Entities;

public class Company : Entity, ITransactionMember
{
    public required string TaxIdentificationNumber { get; set; }
    public required string TaxIdentificationType { get; set; }
    public required string Address { get; set; }
    public decimal Amount { get; set; }
}