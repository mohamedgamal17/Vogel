using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.BuildingBlocks.EntityFramework;
using Vogel.BuildingBlocks.EntityFramework.Interceptors;
using Vogel.Infrastructure.EntityFramework;
namespace Vogel.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterEntityFramework(services, configuration);

            ConfigureS3StorageProvider(services, configuration);
                
            return services;
        }


        private static void RegisterEntityFramework(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp,opt) =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("Default"), op =>
                {
                    op.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

                opt.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptors>());
                opt.AddInterceptors(sp.GetRequiredService<DispatchDomainEventInterceptor>());
            });

            services.AddVogelEfCoreInterceptors();
        }

        private static void ConfigureS3StorageProvider(IServiceCollection services, IConfiguration configuration)
        {
            var s3config = new S3ObjectStorageConfiguration();

            configuration.Bind(S3ObjectStorageConfiguration.CONFIG_KEY , s3config);

            services.AddSingleton(s3config);

            services.AddTransient<IMinioClient, MinioClient>();

            services.AddTransient<IS3ObjectStorageService, S3ObjectStorageService>();
        }

    }
}
