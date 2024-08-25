using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Infrastructure.Modularity;

namespace Vogel.Social.Infrastructure
{
    public class SocialModuleInstaller : IModuleInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddService
        }
    }
}
