namespace Domain.Entities.BankClients;

public enum CompanyType
{
    SoleProprietorship,
    AdditionalLiabilityCompany,
    OpenJointStockCompany,
    ClosedJointStockCompany,
    UnitaryEnterprise,
}

public class Company : BankClient
{
    public CompanyType CompanyType { get; set; }
    public string TaxIdentificationNumber { get; set; }
    public string TaxIdentificationType { get; set; }
    public string Address { get; set; }
}