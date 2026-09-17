using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.Queries.Products;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? categoryId = null)
    {
        var result = await _mediator.Send(new GetProductsQuery(page, pageSize, search, categoryId));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        return product is null
            ? NotFound(new { message = $"Produto com id '{id}' nao foi encontrado." })
            : Ok(product);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var created = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateProductRequest request)
    {
        var command = new UpdateProductCommand(id, request.Name, request.Description, request.Price, request.CategoryId, request.StockQuantity);
        var updated = await _mediator.Send(command);
        return Ok(updated);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }
}

// Request separado do Command aqui porque o Id vem da rota, nao do corpo -
// fica mais claro pra quem consome a API do que reaproveitar o Command com Id vazio.
public record UpdateProductRequest(string Name, string? Description, decimal Price, string CategoryId, int StockQuantity);
