using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Infrastructure.Modularity;

namespace Vogel.BuildingBlocks.Infrastructure.Extensions
{
    public static class HostExtensions
    {
        public static async Task RunModulesBootstrapperAsync(this IHost host)
        {
            var moduleBootstrappers =  host.Services.GetServices<IModuleBootstrapper>();

            foreach(var moduleBootstrap in moduleBootstrappers)
            {
                await moduleBootstrap.Bootstrap(host.Services);
            }
        }
    }
}
