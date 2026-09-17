using MediatR;
using Hypesoft.Application.Common;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Queries.Dashboard;

public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetDashboardSummaryQueryHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var products = (await _productRepository.GetAllAsync()).ToList();
        var categories = (await _categoryRepository.GetAllAsync()).ToList();
        var categoryNames = categories.ToDictionary(c => c.Id, c => c.Name);

        var lowStockProducts = StockCalculator.GetLowStockProducts(products).ToList();

        // agrupamento simples em memoria - o volume de produto desse desafio nao
        // justifica montar uma aggregation pipeline no Mongo pra isso.
        var byCategory = products
            .GroupBy(p => p.CategoryId)
            .Select(g => new CategoryProductCountDto
            {
                CategoryId = g.Key,
                CategoryName = categoryNames.GetValueOrDefault(g.Key, "Sem categoria"),
                ProductCount = g.Count()
            })
            .OrderByDescending(c => c.ProductCount)
            .ToList();

        return new DashboardSummaryDto
        {
            TotalProducts = products.Count,
            TotalStockValue = StockCalculator.CalculateTotalStockValue(products),
            LowStockCount = lowStockProducts.Count,
            LowStockProducts = lowStockProducts
                .Select(p => ProductDto.FromEntity(p, categoryNames.GetValueOrDefault(p.CategoryId)))
                .ToList(),
            ProductsByCategory = byCategory
        };
    }
}
