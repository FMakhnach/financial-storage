using FinancialStorage.Api.Controllers.v1.Requests;
using FinancialStorage.Api.Controllers.v1.Responses;
using FinancialStorage.Api.Mappers;
using FinancialStorage.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinancialStorage.Api.Controllers.v1;

[ApiController]
[Route("/api/v1/dividend")]
[Produces("application/json")]
public class DividendController : ControllerBase
{
    private readonly IDividendService _dividendService;

    public DividendController(IDividendService dividendService)
    {
        _dividendService = dividendService;
    }

    /// <summary>
    /// Get dividend by ticker and moment.
    /// </summary>
    [HttpGet("{ticker}")]
    public async Task<ActionResult<GetDividendsResponse>> GetDividendAsync([FromRoute] string ticker, [FromQuery] DateTimeOffset? moment)
    {
        var dividends = await _dividendService.GetAsync(ticker, moment ?? DateTimeOffset.UtcNow, HttpContext.RequestAborted);

        var response = new GetDividendsResponse
        {
            Items = dividends.Select(x => x.ToResponseItem()).ToArray(),
        };

        return Ok(response);
    }

    /// <summary>
    /// Flexible dividend search
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<SearchDividendsResponse>> SearchDividendsAsync([FromBody] SearchDividendsRequest request)
    {
        if (request is { Start: null, End: not null })
        {
            return BadRequest("'from' can be empty only if 'to' is not empty");
        }

        if (request.Tickers is { Count: 0 })
        {
            return BadRequest("Must provide at least one ticker");
        }

        var model = request.ToSearchModel();

        var dividends = await _dividendService.SearchAsync(model, HttpContext.RequestAborted);

        var response = new SearchDividendsResponse
        {
            Items = dividends.Select(x => x.ToResponseItem()).ToArray(),
        };

        return Ok(response);
    }

    /// <summary>
    /// Add dividend information
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> UpdateDividendsAsync([FromBody] UpdateDividendsRequest request)
    {
        var updateModels = request.Items.Select(x => x.ToUpdateModel()).ToArray();

        await _dividendService.UpdateAsync(updateModels, HttpContext.RequestAborted);

        return Ok();
    }
}