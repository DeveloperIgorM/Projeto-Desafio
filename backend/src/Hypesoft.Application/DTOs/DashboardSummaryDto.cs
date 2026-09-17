namespace Hypesoft.Application.DTOs;

public class DashboardSummaryDto
{
    public int TotalProducts { get; set; }
    public decimal TotalStockValue { get; set; }
    public int LowStockCount { get; set; }
    public List<ProductDto> LowStockProducts { get; set; } = new();
    public List<CategoryProductCountDto> ProductsByCategory { get; set; } = new();
}

public class CategoryProductCountDto
{
    public string CategoryId { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
}
