using MediatR;
using Hypesoft.Application.Common;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Queries.Products;

public record GetLowStockProductsQuery(int Threshold = 10) : IRequest<List<ProductDto>>;

public class GetLowStockProductsQueryHandler : IRequestHandler<GetLowStockProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetLowStockProductsQueryHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<List<ProductDto>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync();
        var lowStock = StockCalculator.GetLowStockProducts(products, request.Threshold);

        var categoryNames = (await _categoryRepository.GetAllAsync()).ToDictionary(c => c.Id, c => c.Name);

        return lowStock
            .Select(p => ProductDto.FromEntity(p, categoryNames.GetValueOrDefault(p.CategoryId)))
            .ToList();
    }
}
