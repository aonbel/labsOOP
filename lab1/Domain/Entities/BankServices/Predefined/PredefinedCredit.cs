namespace Domain.Entities.BankServices.Predefined;

public class PredefinedCredit : PredefinedBankService
{
    public required decimal InterestRate { get; set; }
}