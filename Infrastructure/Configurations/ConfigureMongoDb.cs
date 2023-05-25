using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Infrastructure.Configurations;

public static class ConfigureMongoDb
{
    public static void ConfigureMongo(this IServiceCollection services, MongoDbSettings mongoDbSettings)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            // serialize enums as strings in api responses (e.g. Role)
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        services.AddSingleton<IMongoClient>(serviceProvider => new MongoClient(mongoDbSettings.ConnectionString));
    }
}