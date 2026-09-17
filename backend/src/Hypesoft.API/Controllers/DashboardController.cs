using MediatR;
using Microsoft.AspNetCore.Mvc;
using Hypesoft.Application.Queries.Dashboard;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _mediator.Send(new GetDashboardSummaryQuery());
        return Ok(summary);
    }
}
