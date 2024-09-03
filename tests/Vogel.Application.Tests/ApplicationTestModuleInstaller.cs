using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vogel.Application.Tests.Services;
using Vogel.Application.Tests.Utilites;
using Vogel.BuildingBlocks.Infrastructure;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Application;
using Vogel.BuildingBlocks.EntityFramework;
namespace Vogel.Application.Tests
{
    public class ApplicationTestModuleInstaller : IModuleInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.
                InstallModule<InfrastructureModuleInstaller>(configuration, environment)
                .Replace<IS3ObjectStorageService, FakeS3ObjectService>()
                .Replace<ISecurityContext, FakeSecurityContext>()
                .AddScoped<FakeUserService>();

            services.AddLogging()
                .AddTransient<ILogger, TestOutputLogger>()
                .AddSingleton<ILoggerFactory>(provider => new TestOutputLoggerFactory(true));

            services.RegisterMediatRCommonPibelineBehaviors()
                .RegisterEfCoreInterceptors();

            services.AddAuthorization();
        }

    }
}
