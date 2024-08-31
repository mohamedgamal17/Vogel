using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using NUnit.Framework;
using System.Reflection;

namespace Vogel.Application.Tests
{
    [SetUpFixture]
    public abstract class TestFixture
    {
        protected IServiceProvider ServiceProvider { get; private set; }
        protected IConfiguration Configuration { get; private set; }
        protected IHostEnvironment HostEnvironment { get; private set; }
        protected abstract Task SetupAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment);
        protected abstract Task InitializeAsync(IServiceProvider services);
        protected abstract Task ShutdownAsync(IServiceProvider services);


        [OneTimeSetUp]
        protected virtual async Task BeforeAnyTests()
        {
            var services = new ServiceCollection();
            Configuration = BuildConfiguration();
            HostEnvironment = new HostingEnvironment()
            {
                EnvironmentName = Environments.Development,
                ApplicationName = Assembly.GetExecutingAssembly().FullName!
            };
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton(HostEnvironment);
            await SetupAsync(services, Configuration,HostEnvironment);
            ServiceProvider = BuildServiceProvider(services);
            await InitializeAsync(ServiceProvider);
        }


        [OneTimeTearDown]
        protected virtual async Task TearDownAsync() 
        {
            await ShutdownAsync(ServiceProvider);
        }

        private IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationManager()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", true, true);

            return builder.Build();
        }

        private IServiceProvider BuildServiceProvider(IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);
            return serviceProvider;
        }
    }
}
