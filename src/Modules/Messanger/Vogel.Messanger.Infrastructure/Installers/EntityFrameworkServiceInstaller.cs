using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.EntityFramework.Interceptors;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Infrastructure.EntityFramework;
using Vogel.Messanger.Infrastructure.EntityFramework.Repositories;
namespace Vogel.Messanger.Infrastructure.Installers
{
    public class EntityFrameworkServiceInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddDbContext<MessangerDbContext>((sp, opt) =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("Default"), cfg => cfg
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                   )
                .AddInterceptors(
                        sp.GetRequiredService<AuditableEntityInterceptors>(),
                        sp.GetRequiredService<DispatchDomainEventInterceptor>()
                   );
            });

            services.AddTransient(typeof(IMessangerRepository<>), typeof(MessangerRepository<>));
        }
    }
}
