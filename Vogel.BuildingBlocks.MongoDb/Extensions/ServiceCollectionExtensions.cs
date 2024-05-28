using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb.Configuration;
using Vogel.BuildingBlocks.MongoDb.Migrations;

namespace Vogel.BuildingBlocks.MongoDb.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static VogelMongoDbBuilder AddVogelMongoDb(this IServiceCollection services, Action<MongoDatabaseSettings> opt)
        {
            var settings = new MongoDatabaseSettings();

            opt.Invoke(settings);

            services.AddSingleton(settings);

            RegisterMongoClient(services);

            RegisterMongoDatabase(services);

            services.AddSingleton<IMongoMigrationEngine, MongoMigrationEngine>();

            return new VogelMongoDbBuilder(services);
        }


        private static void RegisterMongoClient(IServiceCollection services)
        {
            services.AddTransient<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<MongoDbSettings>();

                return new MongoClient(settings.ConnectionString);
            });
        }

        private static void RegisterMongoDatabase(IServiceCollection services)
        {
            services.AddTransient<IMongoDatabase>(sp =>
            {
                var settings = sp.GetRequiredService<MongoDbSettings>();

                var client = sp.GetRequiredService<IMongoClient>();

                return client.GetDatabase(settings.Database);
            });
        }
    }
}
