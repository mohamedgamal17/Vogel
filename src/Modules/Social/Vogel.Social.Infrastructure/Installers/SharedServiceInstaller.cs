using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.Social.Presentation.Services;
using Vogel.Social.Shared.Services;

namespace Vogel.Social.Infrastructure.Installers
{
    public class SharedServiceInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddTransient<IUserService, UserService>();
        }
    }
}
