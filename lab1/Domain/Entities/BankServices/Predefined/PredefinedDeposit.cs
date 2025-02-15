namespace Domain.Entities.BankServices.Predefined;

public class PredefinedDeposit : PredefinedBankService
{
    public required decimal InterestRate { get; set; }
}