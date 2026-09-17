using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Infrastructure.Configurations;
using Hypesoft.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Hypesoft.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterMongoClassMaps();

        var mongoSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>() ?? new MongoDbSettings();
        var connectionString = string.IsNullOrWhiteSpace(mongoSettings.ConnectionString) ? "mongodb://localhost:27017" : mongoSettings.ConnectionString;
        var databaseName = string.IsNullOrWhiteSpace(mongoSettings.DatabaseName) ? "hypesoft" : mongoSettings.DatabaseName;

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }

    // O Domain não pode depender do MongoDB.Driver (isso quebraria a Clean Architecture),
    // então em vez de anotar as entidades com [BsonId] etc., eu registro aqui, na Infrastructure,
    // como cada entidade deve ser mapeada pro Mongo - a Id (string) vira o _id do documento.
    private static void RegisterMongoClassMaps()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Product)))
        {
            BsonClassMap.RegisterClassMap<Product>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(p => p.Id);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Category)))
        {
            BsonClassMap.RegisterClassMap<Category>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id);
            });
        }
    }
}
