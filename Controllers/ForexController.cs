using ForexFintechAPI.Data;
using ForexFintechAPI.Models;
using ForexFintechAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ForexFintechAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ForexController : ControllerBase
{
    private IForexService _forexService;
    private readonly MyContext _myContext;
    public ForexController(IForexService forexService,
                            MyContext myContext)
    {
        _forexService = forexService;
        _myContext = myContext;
    }
    [HttpPost]
    [Route("Rates")]
    [Limit(Name = "Ram")]
    [SwaggerOperation(Summary = "Get Latest Rate", Description = "Get's the latest conversion rate for the given currencies from Xe API")]
    [SwaggerResponse(200, Type = typeof(decimal), ContentTypes = new[] { "application/json" })]
    [SwaggerResponse(500, Type = typeof(string), ContentTypes = new[] { "application/json" })]
    public async Task<IActionResult> Rates(string fromCurrency, string toCurrency)
    {
        var result = await _forexService.GetRateAsync(fromCurrency, toCurrency);
        await _myContext.ExchangeRates.AddAsync(result);
        await _myContext.SaveChangesAsync();
        return Ok(result.Exchange_Rate);
    }
}
