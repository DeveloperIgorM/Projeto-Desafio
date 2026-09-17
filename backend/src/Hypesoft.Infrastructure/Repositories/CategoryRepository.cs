using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using MongoDB.Driver;

namespace Hypesoft.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private const string CollectionName = "categories";
    private readonly IMongoCollection<Category> _collection;

    public CategoryRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Category>(CollectionName);
    }

    public async Task<Category?> GetByIdAsync(string id) =>
        await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();

    public async Task<IEnumerable<Category>> GetAllAsync() =>
        await _collection.Find(_ => true).SortBy(c => c.Name).ToListAsync();

    public async Task<Category> CreateAsync(Category category)
    {
        await _collection.InsertOneAsync(category);
        return category;
    }

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(c => c.Id == id);
}
