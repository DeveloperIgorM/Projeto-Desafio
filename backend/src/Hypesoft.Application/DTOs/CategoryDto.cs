using Hypesoft.Domain.Entities;

namespace Hypesoft.Application.DTOs;

public class CategoryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public static CategoryDto FromEntity(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name
    };
}
