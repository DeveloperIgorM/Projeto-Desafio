using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PingController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult Public() => Ok(new { message = "pong (sem autenticacao)" });

    [Authorize]
    [HttpGet("private")]
    public IActionResult Private()
    {
        var username = User.FindFirst("preferred_username")?.Value ?? User.Identity?.Name;
        return Ok(new { message = "pong (autenticado)", user = username });
    }
}
