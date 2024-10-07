using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
namespace Vogel.Messanger.Infrastructure
{
    public class MessangerModuleInstaller : IModuleInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.InstallServiceFromAssembly(configuration, environment, AssemblyReference.Assembly);

            services.AddTransient<IModuleBootstrapper, MessangerModuleBootstrapper>();
        }
    }
}
