using MediatR;
using Microsoft.AspNetCore.Mvc;
using WSBInvestmentPredictor.Prediction.Infrastructure.MarketData;
using WSBInvestmentPredictor.Prediction.MarketData;
using WSBInvestmentPredictor.Prediction.Shared.Queries;

namespace WSBInvestmentPredictor.Prediction.API.Controllers;

[ApiController]
[Route("api/MarketData")]
public class MarketDataController : ControllerBase
{
    private readonly IPolygonClient _polygon;
    private readonly IMediator _mediator;

    public MarketDataController(IPolygonClient polygon, IMediator mediator)
    {
        _polygon = polygon;
        _mediator = mediator;
    }

    [HttpGet("{symbol}")]
    public async Task<IActionResult> Get(string symbol, DateTime? from = null, DateTime? to = null)
    {
        if (from is null || to is null)
        {
            to ??= DateTime.UtcNow.Date;
            from ??= to.Value.AddDays(-100);
        }

        try
        {
            var data = await _polygon.GetDailyOhlcvAsync(symbol, from.Value, to.Value);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("tickers")]
    public async Task<IActionResult> GetTickers()
    {
        var result = await _mediator.Send(new GetSp500TickersQuery());
        return Ok(result);
    }
}