using Hypesoft.Domain.Entities;

namespace Hypesoft.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(string id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category> CreateAsync(Category category);
    Task DeleteAsync(string id);
}
