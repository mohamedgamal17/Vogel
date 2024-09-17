using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.Application.Tests;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.Content.Application.Tests.Fakers;
using Vogel.Content.Infrastructure;
using Vogel.Social.Shared.Services;
namespace Vogel.Content.Application.Tests
{
    public class ContentApplicationTestModuleInstaller : IModuleInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.InstallModule<ContentModuleInstaller>(configuration, environment)
                .InstallModule<ApplicationTestModuleInstaller>(configuration, environment);

            services.AddVogelMongoDb(opt =>
            {
                opt.ConnectionString = configuration.GetValue<string>("MongoDb:ConnectionString")!;
                opt.Database = configuration.GetValue<string>("MongoDb:Database")!;
            });

            services.AddTransient<IUserService, FakeUserService>();
        }
    }
}
