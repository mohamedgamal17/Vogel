using Minio;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.BuildingBlocks.Infrastructure.Security;

namespace Vogel.BuildingBlocks.Infrastructure
{
    public class InfrastructureModuleInstaller : IModuleInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            ConfigureS3StorageProvider(services, configuration);

            ConfigureSecurity(services);
        }

        private static void ConfigureS3StorageProvider(IServiceCollection services, IConfiguration configuration)
        {
            var s3config = new S3ObjectStorageConfiguration();

            configuration.Bind(S3ObjectStorageConfiguration.CONFIG_KEY, s3config);

            services.AddSingleton(s3config);

            services.AddTransient<IMinioClient, MinioClient>();

            services.AddTransient<IS3ObjectStorageService, S3ObjectStorageService>();
        }

        private static void ConfigureSecurity(IServiceCollection services)
        {

            services.AddTransient<IApplicationAuthorizationService, ApplicationAuthorizationService>();

            services.AddTransient<ISecurityContext, SecurityContext>();
        }
    }
}
