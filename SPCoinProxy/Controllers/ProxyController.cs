using Microsoft.AspNetCore.Mvc;
using SPCoinProxy.Services;

namespace SPCoinProxy.Controllers;

[Route("proxy")]
public class ProxyController : ControllerBase
{
    private readonly ISPCoinHandler handler;
    public ProxyController(ISPCoinHandler handler)
    {
        this.handler = handler;
    }

    [HttpGet("balance/get")]
    public Task<int> Get(
        [FromQuery(Name = "uuid")] Guid uuid,
        CancellationToken cancellationToken)
    {
        return handler.GetUserBalance(uuid, cancellationToken);
    }

    [HttpPost("balance/increase")]
    public Task<bool> Increase(
        [FromQuery(Name = "uuid")] Guid uuid,
        [FromQuery(Name = "reason")] string reason,
        [FromQuery(Name = "value")] int value,
        CancellationToken cancellationToken)
    {
        return handler.IncreaseUserBalance(uuid, reason, value, cancellationToken);
    }

    [HttpPost("balance/decrease")]
    public Task<bool> Decrease(
        [FromQuery(Name = "uuid")] Guid uuid,
        [FromQuery(Name = "reason")] string reason,
        [FromQuery(Name = "value")] int value,
        CancellationToken cancellationToken)
    {
        return handler.DecreaseUserBalance(uuid, reason, value, cancellationToken);
    }
}
