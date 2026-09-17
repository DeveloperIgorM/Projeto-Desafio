using MediatR;
using Hypesoft.Application.Common.Exceptions;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Commands.Categories;

public record DeleteCategoryCommand(string Id) : IRequest;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Categoria", request.Id);

        if (await _productRepository.ExistsWithCategoryAsync(category.Id))
        {
            throw new CategoryInUseException(category.Id);
        }

        await _categoryRepository.DeleteAsync(category.Id);
    }
}
