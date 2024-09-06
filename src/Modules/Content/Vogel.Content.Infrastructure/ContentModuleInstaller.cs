using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Modularity;

namespace Vogel.Content.Infrastructure
{
    public class ContentModuleInstaller : IModuleInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.InstallServiceFromAssembly(configuration, environment,
                    Assembly.GetExecutingAssembly()
                );

            services.AddTransient<IModuleBootstrapper, ContentModuleBootstrapper>();
        }
    }
}
