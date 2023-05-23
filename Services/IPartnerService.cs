using ForexFintechAPI.Data;
using ForexFintechAPI.Models;
using IPayServiceReference;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;

namespace ForexFintechAPI.Services;

public interface IPartnerService
{
    Task<List<FetchedPartner>> GetPartnerAsync();
    Task<List<Partner>> GetPartnersByCountryAsync(string countryName);
}
public class PartnerService : IPartnerService
{
    private readonly IConfiguration _configuration;
    private readonly string _endPoint;
    private readonly MyContext _myContext;
    public PartnerService(IConfiguration configuration, MyContext myContext)
    {
        _configuration = configuration;
        _endPoint = _configuration.GetSection("IPayConfig").GetValue<string>("V4SoapUrl");
        _myContext = myContext;
    }
    public async Task<List<FetchedPartner>> GetPartnerAsync()
    {
        var details = new FetchingDetails
        {
            AGENT_CODE = "testSyd",
            USER_ID = "151",
            AGENT_SESSION_ID = "try",
            CATALOGUE_TYPE = "AGT",
            ADDITIONAL_FIELD1 = "India",
            ADDITIONAL_FIELD2 = "",
            ADDITIONAL_FIELD3 = "",
        };
        string apiPassword = "testing@";
        string signature = GenerateSignature(details.AGENT_CODE, details.USER_ID, details.AGENT_SESSION_ID, details.CATALOGUE_TYPE, details.ADDITIONAL_FIELD1, details.ADDITIONAL_FIELD2, details.ADDITIONAL_FIELD3, apiPassword);
        var endpointAddress = new EndpointAddress(_endPoint);
        var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
        using (var client = new iRemitIntWsSendServiceV4SoapClient(binding, endpointAddress))
        {
            var response = await client.GetCatalogueAsync(details.AGENT_CODE, details.USER_ID, details.AGENT_SESSION_ID, details.CATALOGUE_TYPE, details.ADDITIONAL_FIELD1, details.ADDITIONAL_FIELD2, details.ADDITIONAL_FIELD3, signature);
            var agent = response.Body.GetCatalogueResult;
            var partnerList = new List<FetchedPartner>();

            return agent.Select(item => new FetchedPartner
            {
                Name = item.VALUE,
                PartnerCode = item.DATA,
                Email = item.MESSAGE,
                Country = details.ADDITIONAL_FIELD1
            })
                .ToList();
        }
    }
    public static string GenerateSignature(string agentCode, string userId, string agentSessionId, string catalogueType, string additionalField1,
                                           string additionalField2, string additionalField3, string apiPassword)
    {
        string signatureString = $"{agentCode}{userId}{agentSessionId}{catalogueType}{additionalField1}";
        if (!string.IsNullOrEmpty(additionalField2))
        {
            signatureString += additionalField2;
        }
        if (!string.IsNullOrEmpty(additionalField3))
        {
            signatureString += additionalField3;
        }
        signatureString += apiPassword;
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(signatureString));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashedBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
    public async Task<List<Partner>> GetPartnersByCountryAsync(string countryName)
    {
        var result = await _myContext.Partners.Include(x=>x.Country).Where(x=>x.Country.Name==countryName).ToListAsync();
        List<Partner> partners = result.Select(p => new Partner
        {
            PartnerCode = p.PartnerCode,
            Name = p.Name,
            Email = p.Email,
            Country = p.Country,
        }).ToList();
        return partners;
    }
}

