using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Orders.Common.Settings;

namespace Orders.Common.MongoDb;

public static class Extensions
{
    public static IServiceCollection AddMongo(this IServiceCollection collection)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeSerializer(BsonType.String));
        collection.AddSingleton(pr =>
                                {
                                    var configuration = pr.GetService<IConfiguration>()!;
                                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                                    var mongoSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                                    var mongoClient = new MongoClient(mongoSettings.ConnectionString);
                                    return mongoClient.GetDatabase(serviceSettings.ServiceName);
                                });
        return collection;
    }

    public static IServiceCollection AddMongoRepository<T>(this IServiceCollection collection, string collectionName) where T : IEntity
    {
        collection.AddSingleton<IRepository<T>, MongoRepository<T>>(pr =>
                                                                    {
                                                                        var database = pr.GetService<IMongoDatabase>()!;
                                                                        return new MongoRepository<T>(database, collectionName);
                                                                    });
        return collection;
    }

   

 
}