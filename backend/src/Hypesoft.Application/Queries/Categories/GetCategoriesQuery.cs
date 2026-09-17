using MediatR;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Repositories;

namespace Hypesoft.Application.Queries.Categories;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(CategoryDto.FromEntity).ToList();
    }
}
