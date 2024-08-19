using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Infrastructure.Modularity;

namespace Vogel.BuildingBlocks.Infrastructure.Extensions
{
    public static class HostApplicationBuilderExtenions
    {
        public static  IHostApplicationBuilder InstallModule<T>(this IHostApplicationBuilder host) where 
            T : class, IModuleInstaller
        {
            var moduleType = typeof(T);

            var module = ((IModuleInstaller) Activator.CreateInstance(moduleType)!)!;

            module.Install(host.Services, host.Configuration, host.Environment);

            return host;
        }
    }
}
