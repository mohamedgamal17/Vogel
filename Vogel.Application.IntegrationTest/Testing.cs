using Bogus;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Security.Claims;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.IntegrationTest.Extensions;
using Vogel.Application.IntegrationTest.Fakes;
using Vogel.Application.IntegrationTest.Utilites;
using Vogel.Domain;
using Vogel.Domain.Utils;
using Vogel.Infrastructure;
using Vogel.Infrastructure.Presistance;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using System.Reflection.Metadata;
using System.Threading;
namespace Vogel.Application.IntegrationTest
{
    [SetUpFixture]
    public class Testing
    {
        private static IServiceProvider _serviceProvider;
        private static IConfiguration _configuration;
        private static ClaimsPrincipal _currentUser = new ClaimsPrincipal();
        public static IServiceProvider ServiceProvider => _serviceProvider;
        public static IConfiguration Configuration => _configuration;
        public static ClaimsPrincipal? CurrentUser => _currentUser;

        static object _lockObj = new object();

        [OneTimeSetUp]
        public void RunBeforeAnyTest()
        {
            var services = new ServiceCollection();

            _configuration = BuildConfiguration();

            services.AddAuthorization();

            services.AddSingleton(_configuration);

            services.AddApplication(_configuration);

            services.AddInfrastructure(_configuration);

            services.Replace<IS3ObjectStorageService, FakeS3ObjectService>();

            services.AddTransient<ISecurityContext, FakeSecurityContext>();

            services.AddLogging();

            services.AddTransient<ILogger, TestOutputLogger>();

            services.AddSingleton<ILoggerFactory>(provider => new TestOutputLoggerFactory(true));
        
            _serviceProvider = BuildServiceProvider(services);
        }

        private IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationManager()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", true, true)
                 .AddEnvironmentVariables();

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

        [OneTimeTearDown]
        public async Task RunAfterAllTests()
        {
            await EnsureDatebaseDeleted();
        }

        private async Task EnsureDatebaseDeleted()
        {
            var mongoConfiguration = _serviceProvider.GetRequiredService<MongoDbSettings>();

            var mongoClient = _serviceProvider.GetRequiredService<IMongoClient>();

            await mongoClient.DropDatabaseAsync(mongoConfiguration.Database);
        }

        public static async Task<Result<TResponse>> SendAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IApplicationReuest<TResponse>
        {
            using var scope = _serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

            return await mediator.Send(request);
        }

        public static async Task<TEntity> InsertAsync<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            using var scope = _serviceProvider.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<IMongoDbRepository<TEntity>>();

            return await repository.InsertAsync(entity);
        }

        public static async Task<List<TEntity>> InsertManyAsync<TEntity>(List<TEntity> entity)
            where TEntity : Entity
        {
            using var scope = _serviceProvider.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<IMongoDbRepository<TEntity>>();

            return await repository.InsertManyAsync(entity);
        }

        public static async Task<TEntity> UpdateAsync<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            using var scope = _serviceProvider.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<IMongoDbRepository<TEntity>>();

            return await repository.UpdateAsync(entity);
        }

        public static async Task DeleteAsync<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            using var scope = _serviceProvider.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<IMongoDbRepository<TEntity>>();

            await repository.DeleteAsync(entity);
        }

        public static async Task<TEntity> SingleAsync<TEntity>(FilterDefinition<TEntity> filter)
            where TEntity : Entity
        {
            using var scope = _serviceProvider.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<IMongoDbRepository<TEntity>>();

            return await repository.SingleAsync(filter);
        }

        public static async Task<TEntity?> FindByIdAsync<TEntity>(string id)
           where TEntity : Entity
        {
            using var scope = _serviceProvider.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<IMongoDbRepository<TEntity>>();

            return await repository.FindByIdAsync(id);
        }


        public static void RemoveCurrentUser()
        {
            lock (_lockObj)
            {
                _currentUser = null;
            }
        }
        public static async Task RunAsUserAsync()
        {
            Faker faker = new Faker();
            Person fakePerson = faker.Person;
            string id = Guid.NewGuid().ToString();
            string userName = fakePerson.UserName;
            string givenName = fakePerson.FirstName;
            string surName = fakePerson.LastName;
            DateTime birthDate = fakePerson.DateOfBirth;
            await RunAsUserAsync(id, userName, givenName, surName, birthDate);
        }

        public static Task RunAsUserAsync(string id ,string userName , string givenName , string surname, 
            DateTime birthDate)

        {
            lock (_lockObj)
            {
                var principal = new ClaimsPrincipal();

                var identity = new ClaimsIdentity();

                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
                identity.AddClaim(new Claim(ClaimTypes.Name, userName));
                identity.AddClaim(new Claim(ClaimTypes.GivenName, givenName));
                identity.AddClaim(new Claim(ClaimTypes.Surname, surname));
                identity.AddClaim(new Claim(ClaimTypes.DateOfBirth, birthDate.ToString()));

                principal.AddIdentity(identity);

                _currentUser = principal;

                return Task.CompletedTask;
            }
         
        }

    }
}
