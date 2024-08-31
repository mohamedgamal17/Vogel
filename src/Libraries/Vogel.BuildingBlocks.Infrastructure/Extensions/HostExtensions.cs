using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Infrastructure.Modularity;

namespace Vogel.BuildingBlocks.Infrastructure.Extensions
{
    public static class HostExtensions
    {
        public static async Task RunModulesBootstrapperAsync(this IHost host)
        {
            await host.Services.RunModulesBootstrapperAsync();
        }

        public static async Task RunModulesBootstrapperAsync(this IServiceProvider serviceProvider)
        {
            var moduleBootstrappers = serviceProvider.GetServices<IModuleBootstrapper>();
            foreach (var moduleBootstrap in moduleBootstrappers)
            {
                await moduleBootstrap.Bootstrap(serviceProvider);
            }
        }
    }
}
