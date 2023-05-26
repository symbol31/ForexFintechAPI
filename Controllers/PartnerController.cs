using ForexFintechAPI.Action_Filters;
using ForexFintechAPI.Data;
using ForexFintechAPI.Models;
using ForexFintechAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace ForexFintechAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PartnerController : ControllerBase
{
    private readonly MyContext _myContext;
    private readonly IPartnerService _partnerService;
    public PartnerController(MyContext myContext, IPartnerService partnerservice)
    {
        _myContext = myContext;
        _partnerService = partnerservice;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Get's list of Partners from Core System",
                      Description = "Get list of Partners from existing core system using API")]
    [SwaggerResponse(200, Type = typeof(List<FetchedPartner>), ContentTypes = new[] { "application/json" })]
    [SwaggerResponse(400, Type = typeof(string), ContentTypes = new[] { "application/json" })]
    [SwaggerResponse(500, Type = typeof(string), ContentTypes = new[] { "application/json" })]
    public async Task<IActionResult> GetPartner()
    {
        var result = await _partnerService.GetPartnerAsync();
        if (result == null)
        {
            return NotFound();
        }
        var country = result.First().Country;
        var existingCountry = await _myContext.Countries.FirstOrDefaultAsync(c => country.Contains(c.Name));
        if (existingCountry == null)
        {
            existingCountry = new Country
            {
                Name = country
            };
            await _myContext.Countries.AddAsync(existingCountry);
        }
        var existingPartnerCodes = _myContext.Partners.Select(p => p.PartnerCode).ToList();
        var newPartners = result.Where(p => !existingPartnerCodes.Contains(p.PartnerCode));

        if (!newPartners.IsNullOrEmpty())
        {
            var partnersToAdd = newPartners.Select(p => new Partner
            {
                PartnerCode = p.PartnerCode,
                Name = p.Name,
                Country = existingCountry,
            });
            await _myContext.AddRangeAsync(partnersToAdd);
        }
        try
        {
            await _myContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var entry = ex.Entries.Single();
            var databaseValues = entry.GetDatabaseValues();
            var clientValues = entry.CurrentValues;
            var errorMessage = "Concurrency conflict detected for entity " + entry.Entity.GetType().Name;
            return BadRequest(errorMessage);
        }
        catch (DbUpdateException ex)
        {
            var errorMessage = ex.InnerException.Message;
            return BadRequest(errorMessage);
        }
        return Ok(result);
    }
    //[ServiceFilter(typeof(ValidationFilter<>))]
    [ServiceFilter(typeof(ValidationFilter))]
    [HttpPost("Manually")]
    [SwaggerOperation(Summary = "Add Partners to the System Manually", Description = "Add Partners to the System Manually who couldn't be fetched from core system or who are new")]
    [SwaggerResponse(200, Type = typeof(PartnerDto), ContentTypes = new[] { "application/json" })]
    [SwaggerResponse(500, Type = typeof(string), ContentTypes = new[] { "application/json" })]
    public async Task<IActionResult> AddPartner([FromBody] PartnerDto partnerDto)
    {
        var country = _myContext.Countries.Where(x => x.Name == partnerDto.Country).FirstOrDefault();
        if (country == null)
        {
            country = new Country
            {
                Name = partnerDto.Country
            };
            _myContext.Countries.Add(country);
        }
        var partner = new Partner
        {
            Name = partnerDto.Name,
            Email = partnerDto.Email,
            PartnerCode = partnerDto.PartnerCode,
            Country = country
        };
        var result = await _myContext.AddAsync(partner);
        await _myContext.SaveChangesAsync();
        return Ok(partner);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get list of Partner in a Country",
                      Description = "Get list of Partners on the basis of the Country specified in the input")]
    [SwaggerResponse(200, Type = typeof(List<PartnerDto>), ContentTypes = new[] { "application/json" })]
    [SwaggerResponse(500, Type = typeof(string), ContentTypes = new[] { "application/json" })]
    public IActionResult GetPartnerByCountry(string country)
    {
        var result = _myContext.Partners.Where(x => x.Country.Name == country).ToList();

        List<PartnerDto> partnersInCountry = result.Select(p => new PartnerDto
        {
            PartnerCode = p.PartnerCode,
            Name = p.Name,
            Email = p.Email,
            Country = country,
        }).ToList();
        return Ok(partnersInCountry);
    }
}