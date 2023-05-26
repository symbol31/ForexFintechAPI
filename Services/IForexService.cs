using ForexFintechAPI.Models;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;

namespace ForexFintechAPI.Services;

public interface IForexService
{
    Task<ExchangeRate> GetRateAsync(string fromCurrency, string toCurrency);
}

public class ForexService : IForexService
{
    private readonly HttpClient _httpclient;
    private readonly XEAPIConfiguration _XEAPIconfig;

    public ForexService( HttpClient client,
                         IOptions<XEAPIConfiguration> XEAPIconfig)
    {
        _httpclient = client;
        _XEAPIconfig = XEAPIconfig.Value;
    }
    public async Task<ExchangeRate> GetRateAsync(string fromCurrency, string toCurrency)
    {
        string url = $"https://xecdapi.xe.com/v1/convert_from.json/?from={fromCurrency}&to={toCurrency}";
        var _accountId = _XEAPIconfig.AccountId;
        var _apiKey = _XEAPIconfig.ApiKey;
        using (var request = new HttpRequestMessage(HttpMethod.Get, url))
        {
            string authHeader = $"{_accountId}:{_apiKey}";
            byte[] authHeaderBytes = Encoding.ASCII.GetBytes(authHeader);
            string encodedAuthHeader = Convert.ToBase64String(authHeaderBytes);
            request.Headers.Add("Authorization", $"Basic {encodedAuthHeader}");
            
                using (var response = await _httpclient.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                    throw new Exception("Something went wrong with Xe API");
                    }
                    var result = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    FetchedData responseObj = JsonSerializer.Deserialize<FetchedData>(result, options);
                    var converted = responseObj.To[0];
                    var rate = converted.Mid;
                    var ExchangeRate = new ExchangeRate
                    {
                        Source_Currency = fromCurrency,
                        Target_Currency = toCurrency,
                        Exchange_Rate = rate,
                        Timestamp = DateTime.Now,
                        Api_Key = _apiKey
                    };
                    return ExchangeRate;
                }
        }
    }
}
