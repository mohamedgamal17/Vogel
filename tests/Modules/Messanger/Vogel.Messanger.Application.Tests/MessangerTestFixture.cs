using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.Application.Tests;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
namespace Vogel.Messanger.Application.Tests
{
    public class MessangerTestFixture : TestFixture
    {
        protected override Task SetupAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.InstallModule<MessangerApplicationTestModuleInstaller>(configuration, hostEnvironment);
            return Task.CompletedTask;
        }
        protected override async Task InitializeAsync(IServiceProvider services)
        {
            await services.RunModulesBootstrapperAsync();
        }
        protected override async Task ShutdownAsync(IServiceProvider services)
        {
            await DropSqlDb();
            await DropMongoDb();
        }
    }
}
