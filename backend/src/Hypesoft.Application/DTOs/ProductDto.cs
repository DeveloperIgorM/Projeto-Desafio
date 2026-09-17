using Hypesoft.Domain.Entities;

namespace Hypesoft.Application.DTOs;

public class ProductDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public int StockQuantity { get; set; }
    public bool IsLowStock { get; set; }

    public static ProductDto FromEntity(Product product, string? categoryName = null) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        CategoryId = product.CategoryId,
        CategoryName = categoryName,
        StockQuantity = product.StockQuantity,
        IsLowStock = product.IsLowStock()
    };
}
