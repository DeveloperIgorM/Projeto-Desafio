using FluentValidation;
using MediatR;
using Hypesoft.Application.Common.Exceptions;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Commands.Products;

public record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    string CategoryId,
    int StockQuantity) : IRequest<ProductDto>;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
    }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // valido se a categoria existe antes de criar o produto - evita produto "orfao"
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId)
            ?? throw new NotFoundException("Categoria", request.CategoryId);

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
            StockQuantity = request.StockQuantity
        };

        var created = await _productRepository.CreateAsync(product);
        return ProductDto.FromEntity(created, category.Name);
    }
}
