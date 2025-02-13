namespace Domain.Entities;

public enum CompanyType
{
    SoleProprietorship,
    AdditionalLiabilityCompany,
    OpenJointStockCompany,
    ClosedJointStockCompany,
    UnitaryEnterprise,
}

public class Company : BankUser
{
    public required CompanyType CompanyType { get; set; }
    public required string TaxIdentificationNumber { get; set; }
    public required string TaxIdentificationType { get; set; }
    public required string Address { get; set; }
}