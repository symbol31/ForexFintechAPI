namespace ForexFintechAPI.Models;

public class ExchangeRate
{
    public int Id { get; set; }
    public string Source_Currency { get; set; }
    public string Target_Currency { get; set; }
    public decimal Exchange_Rate { get; set; }
    public decimal Service_Fee { get; set; }
    public DateTime Timestamp { get; set; }
    public string Api_Key { get; set; }
    public int Cache_Expiry { get; set; }
}

public class FetchedData
{
    public string Terms { get; set; }
    public string Privacy { get; set; }
    public string From { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public List<Conversion> To { get; set; }
}
public class Conversion
{
    public string QuoteCurrency { get; set; }
    public decimal Mid { get; set; }
}
