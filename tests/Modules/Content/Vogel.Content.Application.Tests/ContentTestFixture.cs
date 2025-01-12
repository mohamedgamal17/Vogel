using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.Application.Tests;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Content.Application.Tests.Fakers;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;

namespace Vogel.Content.Application.Tests
{
    [TestFixture]
    public class ContentTestFixture : TestFixture
    {
        protected FakeUserService UserService { get; }

        public ContentTestFixture()
        {
            UserService = ServiceProvider.GetRequiredService<FakeUserService>();
        }
        protected override Task SetupAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.InstallModule<ContentApplicationTestModuleInstaller>(configuration, hostEnvironment);
            return Task.CompletedTask;
        }
        protected override async Task InitializeAsync(IServiceProvider services)
        {
            await services.RunModulesBootstrapperAsync();

            await SeedData(services);
        }

        private Task SeedData(IServiceProvider services)
        {
            var users = SeedUsers(services);
            return Task.CompletedTask;
        }

        private Task<List<UserDto>> SeedUsers(IServiceProvider services)
        {
            var userService = services.GetRequiredService<FakeUserService>();

            var users = new UserFaker().Generate(50);

            userService.AddRangeOfUsers(users);

            return Task.FromResult(users);
        }
        protected override async Task ShutdownAsync(IServiceProvider services)
        {
            await DropSqlDb();
            await DropMongoDb();
        }
    }
}
