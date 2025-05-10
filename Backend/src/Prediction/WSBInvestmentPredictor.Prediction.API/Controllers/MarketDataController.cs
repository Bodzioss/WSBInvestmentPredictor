using Microsoft.AspNetCore.Mvc;
using WSBInvestmentPredictor.Prediction.MarketData;

namespace WSBInvestmentPredictor.Prediction.API.Controllers;

[ApiController]
[Route("api/MarketData")]
public class MarketDataController : ControllerBase
{
    private readonly IPolygonClient _polygon;

    public MarketDataController(IPolygonClient polygon)
    {
        _polygon = polygon;
    }

    [HttpGet("{symbol}")]
    public async Task<IActionResult> Get(string symbol, DateTime? from = null, DateTime? to = null)
    {
        from ??= DateTime.UtcNow.AddDays(-360);
        to ??= DateTime.UtcNow;

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
}