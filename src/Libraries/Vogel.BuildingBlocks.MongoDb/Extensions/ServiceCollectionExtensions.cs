﻿using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb.Configuration;
using Vogel.BuildingBlocks.MongoDb.Migrations;

namespace Vogel.BuildingBlocks.MongoDb.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static VogelMongoDbBuilder AddVogelMongoDb(this IServiceCollection services, Action<MongoDbSettings> opt)
        {
            var settings = new MongoDbSettings();

            opt.Invoke(settings);

            services.AddSingleton(settings);

            RegisterMongoClient(services);

            RegisterMongoDatabase(services);

            services.AddSingleton<MongoMigrationEngine>();

            services.AddSingleton<IMongoMigrationEngine, MongoMigrationEngine>();

            services.AddTransient(typeof(IMongoRepository<>), typeof(MongoRepository<>));

            return new VogelMongoDbBuilder(services);
        }


        private static void RegisterMongoClient(IServiceCollection services)
        {
            services.AddTransient<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<MongoDbSettings>();

                var clientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);

                clientSettings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;

                return new MongoClient(clientSettings);
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