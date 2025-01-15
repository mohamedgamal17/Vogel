 using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using MongoDB.Driver;
using NUnit.Framework;
using Respawn;
using Respawn.Graph;
using System;
using System.Reflection;
using Vogel.Application.Tests.Services;
using Vogel.BuildingBlocks.MongoDb.Configuration;

namespace Vogel.Application.Tests
{
    [SetUpFixture]
    public abstract class TestFixture
    {
        protected IServiceProvider ServiceProvider { get; private set; }
        protected IConfiguration Configuration { get; private set; }
        protected IHostEnvironment HostEnvironment { get; private set; }
        protected IMediator Mediator { get; private set; }
        protected FakeAuthenticationService AuthenticationService { get; private set; }
        protected abstract Task SetupAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment);
        protected abstract Task InitializeAsync(IServiceProvider services);
        protected abstract Task ShutdownAsync(IServiceProvider services);

        protected TestFixture()
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
            SetupAsync(services, Configuration, HostEnvironment).Wait();
            ServiceProvider = BuildServiceProvider(services);
        }


        [OneTimeSetUp]
        protected virtual async Task BeforeAnyTests()
        {        
            Mediator = ServiceProvider.GetRequiredService<IMediator>();
            AuthenticationService = ServiceProvider.GetRequiredService<FakeAuthenticationService>();
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



        protected async Task DropMongoDb(IServiceProvider services)
        {
            var mongoConfiguration = ServiceProvider.GetRequiredService<MongoDbSettings>();

            var mongoClient = ServiceProvider.GetRequiredService<IMongoClient>();

            await mongoClient.DropDatabaseAsync(mongoConfiguration.Database);
        }
    }
}
