using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Infrastructure.Modularity;

namespace Vogel.BuildingBlocks.Infrastructure.Extensions
{
    public static class HostExtensions
    {
        public static async Task Bootstrap<TModule>(this IHost host) where TModule : class , IModuleBootstrapper
        {
            var type = typeof(TModule);

            var obj = Activator.CreateInstance(type)!;

            var module = ((IModuleBootstrapper)obj)!;

            await module.Bootstrap(host.Services);
        }
    }
}
