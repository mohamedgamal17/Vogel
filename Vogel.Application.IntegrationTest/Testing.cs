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
using Vogel.Infrastructure;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Vogel.BuildingBlocks.Domain;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.Application.Uow;
using System.Linq.Expressions;
using Vogel.BuildingBlocks.MongoDb.Configuration;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.MongoDb.Migrations;
using Vogel.Domain.Users;
using Bogus.DataSets;
using Vogel.Domain.Medias;
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
        public async Task  RunBeforeAnyTest()
        {
            var services = new ServiceCollection();

            _configuration = BuildConfiguration();

            services.AddAuthorizationCore();

            services.AddSingleton(_configuration);

            services.AddApplication(_configuration);

            services.AddInfrastructure(_configuration);

            services.Replace<IS3ObjectStorageService, FakeS3ObjectService>();

            services.Replace<ISecurityContext, FakeSecurityContext>();

            services.AddLogging();

            services.AddTransient<ILogger, TestOutputLogger>();

            services.AddSingleton<ILoggerFactory>(provider => new TestOutputLoggerFactory(true));
        
            _serviceProvider = BuildServiceProvider(services);

            await ApplyEFCoreMigration(_serviceProvider);

            await ApplyMongoDbMigration(_serviceProvider);
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


        private async Task ApplyEFCoreMigration(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<DbContext>();

            await dbContext.Database.MigrateAsync();
        }

        private async Task ApplyMongoDbMigration(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var migrationEngine = scope.ServiceProvider.GetRequiredService<IMongoMigrationEngine>();

            await migrationEngine.MigrateAsync();
        }

        [OneTimeTearDown]
        public async Task RunAfterAllTests()
        {
        //    await DropSqlDb();
         //   await DropMongoDb();
        }

        private async Task DropSqlDb()
        {
            var dbContext = _serviceProvider.GetRequiredService<DbContext>();

            await dbContext.Database.EnsureDeletedAsync();
        }

        private async Task DropMongoDb()
        {
            var mongoConfiguration = _serviceProvider.GetRequiredService<MongoDbSettings>();

            var mongoClient = _serviceProvider.GetRequiredService<IMongoClient>();

            await mongoClient.DropDatabaseAsync(mongoConfiguration.Database);
        }

        public static async Task<Result<TResponse>> SendAsync<TResponse>(IApplicationReuest<TResponse> request)
        {
            using var scope = _serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

            return await mediator.Send(request);
        }

        public static async Task<TEntity> InsertAsync<TEntity>(TEntity entity)
            where TEntity : class,IEntity
        {
            return await WithUnitOfWork(async (sp) =>
            {
                var repository = sp.GetRequiredService<IRepository<TEntity>>();
                return await repository.InsertAsync(entity);
            });
        }

        public static async Task<List<TEntity>> InsertManyAsync<TEntity>(List<TEntity> entity)
            where TEntity : class, IEntity
        {
            return await WithUnitOfWork(async (sp) =>
            {
                var repository = sp.GetRequiredService<IRepository<TEntity>>();

                return await repository.InsertManyAsync(entity);
            });
        }

        public static async Task<TEntity> UpdateAsync<TEntity>(TEntity entity)
            where TEntity : class ,IEntity
        {
            return await WithUnitOfWork(async (sp) =>
            {
                var repository = sp.GetRequiredService<IRepository<TEntity>>();
                return await repository.UpdateAsync(entity);
            });
        }

        public static async Task DeleteAsync<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            await WithUnitOfWork(async (sp) =>
            {
                var repository = sp.GetRequiredService<IRepository<TEntity>>();

                await repository.DeleteAsync(entity);

                return Unit.Value;

            });

        }

        public static async Task<TEntity> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> filter)
            where TEntity : class, IEntity
        {
            using var scope = _serviceProvider.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<IRepository<TEntity>>();

            return await repository.SingleAsync(filter);
        }

        public static async Task<TEntity?> FindByIdAsync<TEntity>(string id)
           where TEntity : class , IEntity
        {
            using var scope = _serviceProvider.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<IRepository<TEntity>>();

            return await repository.FindByIdAsync(id);
        }


        public static async Task<TResult> WithUnitOfWork<TResult>(Func<IServiceProvider,Task<TResult>> func)
        {
            using var scope = _serviceProvider.CreateScope();

            var unitOfWorkManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            var uow = await unitOfWorkManager.BeginAsync();

            var result = await func(scope.ServiceProvider);

            await uow.CommitAsync();

            return result;
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

        public static async Task RunAsUserWithProfile()
        {
            await RunAsUserAsync();

            await InsertAsync(new UserAggregate
            {
                Id = CurrentUser?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? Guid.NewGuid().ToString(),
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                BirthDate = DateTime.Now,
                Gender = Domain.Users.Gender.Male,
            });
        }

        public static async Task RunAsUserAsync(string id)
        {
            Faker faker = new Faker();
            Person fakePerson = faker.Person;
            string userId  = id;
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
