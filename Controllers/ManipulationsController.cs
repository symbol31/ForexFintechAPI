using ForexFintechAPI.Action_Filters;
using ForexFintechAPI.Data;
using ForexFintechAPI.Models;
using ForexFintechAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace ForexFintechAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ManipulationsController : ControllerBase
{
    private readonly MyContext _myContext;
    private readonly IForexService _forexService;
    public ManipulationsController(MyContext myContext,
                                   IForexService forexService)
    {
        _myContext = myContext;
        _forexService = forexService;
    }
    [ServiceFilter(typeof(ValidationFilter))]
    [HttpPost]
    [Authorize(Policy = "CanAccessExchangeRate")]
    [SwaggerOperation(Summary = "Update Margin,Discount and Premium", Description = "Update Margin,Discount and Premium for individual partner by using their PartnerId")]
    [SwaggerResponse(200, Type = typeof(decimal), ContentTypes = new[] { "application/json" })]
    [SwaggerResponse(500, Type = typeof(string), ContentTypes = new[] { "application/json" })]
    public async Task<IActionResult> PostManipulation(string partnerCode, [FromBody]ExchangeManipulationDataDto manipulationData)
    {
        var partner = await _myContext.Partners.FirstOrDefaultAsync(x => x.PartnerCode == partnerCode);
        if (partner == null)
            return NotFound("Partner with given Partner Code doesn't exist in database.");

            var exchangeRate = await _myContext.ExchangeRates
                                               .Where(e => e.Source_Currency == manipulationData.SourceCurrency &&
                                                           e.Target_Currency == manipulationData.TargetCurrency)
                                               .FirstOrDefaultAsync();
            if (exchangeRate == null)
            {
                exchangeRate = await _forexService.GetRateAsync(manipulationData.SourceCurrency, manipulationData.TargetCurrency);
                await _myContext.ExchangeRates.AddAsync(exchangeRate);
            }

            decimal rate = exchangeRate.Exchange_Rate;
            var ExchangeManipulationData = new ExchangeManipulationData
            {
                Margin = manipulationData.Margin,
                Premium = manipulationData.Premium,
                Discount = manipulationData.Discount,
                Rate = rate,
                Amount = rate - (manipulationData.Margin + manipulationData.Premium - manipulationData.Discount),
                Partner = partner,
                SourceCurrency = manipulationData.SourceCurrency,
                TargetCurrency = manipulationData.TargetCurrency
            };
            await _myContext.AddAsync(ExchangeManipulationData);
            await _myContext.SaveChangesAsync();
            return Ok(ExchangeManipulationData.Amount);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get Margin,Discount and Premium of Partner",
                      Description = "Get list of Partners with their final exchange rates for specific Source to Target Currency")]
    [SwaggerResponse(200, Type = typeof(ExchangeManipulationData), ContentTypes = new[] { "application/json" })]
    [SwaggerResponse(400, "Problem fetching information from database", ContentTypes = new[] { "application/json" })]
    [SwaggerResponse(500, Type = typeof(string), ContentTypes = new[] { "application/json" })]
    public async Task<IActionResult> GetManipulations()
    {
            var transaction = await _myContext
                                    .ExchangeManipulationData
                                    .Select(t => new
                                    {
                                        t.Id,
                                        t.Amount,
                                        t.SourceCurrency,
                                        t.TargetCurrency,
                                        Partner = new
                                        {
                                            t.Partner.PartnerCode,
                                            t.Partner.Name,
                                            Country = new
                                            {
                                                t.Partner.Country.Name
                                            }
                                        }
                                    })
                                   .ToListAsync();

            if (transaction == null)
            {
                return BadRequest();
            }
            return Ok(transaction);
    }
}
