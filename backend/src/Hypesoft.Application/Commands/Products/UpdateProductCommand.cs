using FluentValidation;
using MediatR;
using Hypesoft.Application.Common.Exceptions;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Commands.Products;

public record UpdateProductCommand(
    string Id,
    string Name,
    string? Description,
    decimal Price,
    string CategoryId,
    int StockQuantity) : IRequest<ProductDto>;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
    }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Produto", request.Id);

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId)
            ?? throw new NotFoundException("Categoria", request.CategoryId);

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;
        product.StockQuantity = request.StockQuantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        return ProductDto.FromEntity(product, category.Name);
    }
}
