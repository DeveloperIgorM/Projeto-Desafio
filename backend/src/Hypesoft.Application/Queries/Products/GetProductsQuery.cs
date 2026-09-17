using MediatR;
using Hypesoft.Application.Common;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Queries.Products;

public record GetProductsQuery(int Page = 1, int PageSize = 10, string? Search = null, string? CategoryId = null)
    : IRequest<PagedResult<ProductDto>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetProductsQueryHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        var (items, totalCount) = await _productRepository.GetPagedAsync(page, pageSize, request.Search, request.CategoryId);

        // isso aqui é o "lookup" que substitui o JOIN que eu faria no MySQL:
        // busco todas as categorias de uma vez e monto um dicionario pra não ficar
        // indo no banco categoria por categoria dentro do loop.
        var categoryNames = (await _categoryRepository.GetAllAsync()).ToDictionary(c => c.Id, c => c.Name);

        var dtos = items
            .Select(p => ProductDto.FromEntity(p, categoryNames.GetValueOrDefault(p.CategoryId)))
            .ToList();

        return new PagedResult<ProductDto>
        {
            Items = dtos,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
