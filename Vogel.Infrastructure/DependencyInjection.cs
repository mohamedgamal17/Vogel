﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.BuildingBlocks.EntityFramework;
using Vogel.Infrastructure.Auhtorization;
using Vogel.Infrastructure.EntityFramework;
using Vogel.Infrastructure.Presistance;
using Vogel.Infrastructure.Presistance.Repositories;
namespace Vogel.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterEntityFramework(services, configuration);

            RegisterMongoDb(services, configuration);

            ConfigureS3StorageProvider(services, configuration);

            services.AddTransient<IApplicationAuthorizationService, ApplicationAuthorizationService>();
                
            return services;
        }


        private static void RegisterEntityFramework(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("Default"), op =>
                {
                    op.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
            });

            services.AddVogelEfCore();
        }

        private static void RegisterMongoDb(IServiceCollection services,IConfiguration configuration)
        {
            var mongoDbSettings = new MongoDbSettings();

            configuration.Bind(MongoDbSettings.CONFIG_KEY, mongoDbSettings);

            services.AddSingleton(mongoDbSettings);

            services.AddScoped<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<MongoDbSettings>();

                return new MongoClient(settings.ConnectionString);
            });

            services.AddScoped(sp =>
            {
                var settings = sp.GetRequiredService<MongoDbSettings>();

                var client = sp.GetRequiredService<IMongoClient>();

                var dbContext = new MongoDbContext(client, sp, settings);

                dbContext.Initialize();

                return dbContext;
            });

        }

        private static void AddMongoDb(IServiceCollection services, IConfiguration configuration)
        {
            var mongoDbSettings = new MongoDbSettings();

            configuration.Bind(MongoDbSettings.CONFIG_KEY, mongoDbSettings);

            services.AddSingleton(mongoDbSettings);

            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<MongoDbSettings>();

                return new MongoClient(settings.ConnectionString);
            });

            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<MongoDbSettings>();

                var client = sp.GetRequiredService<IMongoClient>();

                var dbContext = new MongoDbContext(client, sp, settings);

                dbContext.Initialize();

                return dbContext;
            });

            services.AddTransient(typeof(IMongoDbRepository<>), typeof(MongoDbRepository<>));
        }

        private static void ConfigureS3StorageProvider(IServiceCollection services, IConfiguration configuration)
        {
            var s3config = new S3ObjectStorageConfiguration();

            configuration.Bind(S3ObjectStorageConfiguration.CONFIG_KEY , s3config);

            services.AddSingleton(s3config);

            services.AddTransient<IMinioClient, MinioClient>();

            services.AddTransient<IS3ObjectStorageService, S3ObjectStorageService>();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<ICommentRepository, CommentRepository>();
        }
    }
}
