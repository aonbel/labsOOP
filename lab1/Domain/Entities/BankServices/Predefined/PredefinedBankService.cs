using Domain.Entities.Core;

namespace Domain.Entities.BankServices.Predefined;

public class PredefinedBankService : Entity
{
    public required int TermInMonths  { get; set; }
    public required string Description  { get; set; }
}