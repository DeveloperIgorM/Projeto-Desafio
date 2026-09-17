using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Hypesoft.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private const string CollectionName = "products";
    private readonly IMongoCollection<Product> _collection;

    public ProductRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Product>(CollectionName);
    }

    public async Task<Product?> GetByIdAsync(string id) =>
        await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<(IEnumerable<Product> Items, long TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, string? categoryId)
    {
        var filterBuilder = Builders<Product>.Filter;
        var filter = filterBuilder.Empty;

        if (!string.IsNullOrWhiteSpace(search))
        {
            filter &= filterBuilder.Regex(p => p.Name, new BsonRegularExpression(search, "i"));
        }

        if (!string.IsNullOrWhiteSpace(categoryId))
        {
            filter &= filterBuilder.Eq(p => p.CategoryId, categoryId);
        }

        var totalCount = await _collection.CountDocumentsAsync(filter);

        var items = await _collection.Find(filter)
            .SortByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        await _collection.InsertOneAsync(product);
        return product;
    }

    public async Task UpdateAsync(Product product) =>
        await _collection.ReplaceOneAsync(p => p.Id == product.Id, product);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(p => p.Id == id);

    public async Task<bool> ExistsWithCategoryAsync(string categoryId) =>
        await _collection.Find(p => p.CategoryId == categoryId).AnyAsync();
}
