using FinancialStorage.Api.Controllers.v1.Requests;
using FinancialStorage.Api.Controllers.v1.Responses;
using FinancialStorage.Api.Mappers;
using FinancialStorage.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinancialStorage.Api.Controllers.v1;

[ApiController]
[Route("/api/v1/key-rate")]
[Produces("application/json")]
public class KeyRateController : ControllerBase
{
    private readonly IKeyRateService _keyRateService;

    public KeyRateController(IKeyRateService keyRateService)
    {
        _keyRateService = keyRateService;
    }

    /// <summary>
    /// Get key rate by country and moment.
    /// </summary>
    [HttpGet("{country}")]
    public async Task<ActionResult<GetKeyRatesResponse>> GetKeyRateAsync([FromRoute] string country, [FromQuery] DateTimeOffset? moment)
    {
        var keyRates = await _keyRateService.GetAsync(country, moment ?? DateTimeOffset.UtcNow, HttpContext.RequestAborted);

        var response = new GetKeyRatesResponse
        {
            Items = keyRates.Select(x => x.ToResponseItem()).ToArray(),
        };

        return Ok(response);
    }

    /// <summary>
    /// Flexible key rate search
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<SearchKeyRatesResponse>> SearchKeyRatesAsync([FromBody] SearchKeyRatesRequest request)
    {
        if (request is { Start: null, End: not null })
        {
            return BadRequest("'from' can be empty only if 'to' is not empty");
        }

        if (request.Countries is { Count: 0 })
        {
            return BadRequest("Must provide at least one country");
        }

        var model = request.ToSearchModel();

        var keyRates = await _keyRateService.SearchAsync(model, HttpContext.RequestAborted);

        var response = new SearchKeyRatesResponse
        {
            Items = keyRates.Select(x => x.ToResponseItem()).ToArray(),
        };

        return Ok(response);
    }

    /// <summary>
    /// Add key rate information
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> UpdateKeyRatesAsync([FromBody] UpdateKeyRatesRequest request)
    {
        var updateModels = request.Items.Select(x => x.ToUpdateModel()).ToArray();

        await _keyRateService.UpdateAsync(updateModels, HttpContext.RequestAborted);

        return Ok();
    }
}