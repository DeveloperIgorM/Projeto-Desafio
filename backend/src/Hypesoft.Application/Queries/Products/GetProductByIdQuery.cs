using MediatR;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Queries.Products;

public record GetProductByIdQuery(string Id) : IRequest<ProductDto?>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product is null)
        {
            return null;
        }

        var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
        return ProductDto.FromEntity(product, category?.Name);
    }
}
