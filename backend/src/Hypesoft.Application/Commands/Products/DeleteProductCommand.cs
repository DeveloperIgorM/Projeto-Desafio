using MediatR;
using Hypesoft.Application.Common.Exceptions;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Commands.Products;

public record DeleteProductCommand(string Id) : IRequest;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Produto", request.Id);

        await _productRepository.DeleteAsync(product.Id);
    }
}
