using Hypesoft.Domain.Entities;

namespace Hypesoft.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(string id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<(IEnumerable<Product> Items, long TotalCount)> GetPagedAsync(int page, int pageSize, string? search, string? categoryId);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(string id);
    Task<bool> ExistsWithCategoryAsync(string categoryId);
}
