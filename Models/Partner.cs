namespace ForexFintechAPI.Models;
public class Partner 
{
    public int Id { get; set; }
    public string PartnerCode { get; set; }
    public string Name { get; set; }
    public string? Email { get; set; }
    public Country Country { get; set; }
    public List<ExchangeManipulationData> ExchangeManipulationData { get; set; }
}
public class PartnerDto : IValidatableModel
{
    public string PartnerCode { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
}

public class ExchangeManipulationData : IValidatableModel
{
    public int Id { get; set; }
    public Partner Partner { get; set; }
    public decimal Rate { get; set; }
    public decimal Margin { get; set; }
    public decimal Discount { get; set; }
    public decimal Premium { get; set; }
    public decimal Amount { get; set; }
    public string SourceCurrency { get; set; }
    public string TargetCurrency { get; set; }
}
public class ExchangeManipulationDataDto  : IValidatableModel
{
    public decimal Margin { get; set; }
    public decimal Discount { get; set; }
    public decimal Premium { get; set; }
    public string SourceCurrency { get; set; }
    public string TargetCurrency { get; set; }
}


public class Country : IValidatableModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Partner> Partner { get; set; }
}

public class FetchedPartner
{
    public int Id { get; set; }
    public string PartnerCode { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
}

public interface IValidatableModel // marker interface
{

}