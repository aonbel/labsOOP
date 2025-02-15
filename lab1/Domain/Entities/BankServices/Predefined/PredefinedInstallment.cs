namespace Domain.Entities.BankServices.Predefined;

public class PredefinedInstallment : PredefinedBankService
{
    public required decimal InterestRate { get; set; }
}