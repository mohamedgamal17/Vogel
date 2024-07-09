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

        private static ClaimsPrincipal? _currentUser = null;

        private static UserAggregate? _currentUserProfile = null;
        public static IServiceProvider ServiceProvider => _serviceProvider;
        public static IConfiguration Configuration => _configuration;
        public static ClaimsPrincipal? CurrentUser => _currentUser;

        public static UserAggregate? CurrentUserProfile => _currentUserProfile;

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
            await DropSqlDb();
            await DropMongoDb();
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

            try
            {
 
                var result = await func(scope.ServiceProvider);

                await uow.CommitAsync();

                return result;
            }
            catch(Exception exception)
            {
                await uow.RollbackAsync(CancellationToken.None);

                throw;
            }
     

            
        }
        public static void RemoveCurrentUser()
        {
            lock (_lockObj)
            {
                _currentUser = null;
                _currentUserProfile = null;
            }
        }

        public static async Task RunAsUserAsync()
        {
            await RunAsUserAsync(Guid.NewGuid().ToString());
        }

        public static async Task RunAsUserAsync(string id)
        {
            var user = await InsertAsync(new UserAggregate
            {
                Id = id,
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                BirthDate = DateTime.Now,
                Gender = Domain.Users.Gender.Male,
            });

            var principal = PrepareUserClaimsPrincipal(user.Id, Guid.NewGuid().ToString(), user.FirstName, user.LastName, user.BirthDate);


            lock (_lockObj)
            {
                _currentUser = principal;
                _currentUserProfile = user;
            }
        }

        public static async Task RunAsUserWithoutProfileAsync()
        {
            await RunAsUserWithoutProfileAsync(Guid.NewGuid().ToString());
        }
        public static Task RunAsUserWithoutProfileAsync(string id)
        {
            var principal = PrepareUserClaimsPrincipal(id, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),Guid.NewGuid().ToString(), DateTime.Now.AddYears(-18));

            lock (_lockObj)
            {
                _currentUser = principal;
            }

            return Task.CompletedTask;
        }

        private static ClaimsPrincipal PrepareUserClaimsPrincipal(string id, string userName, string givenName, string surname,
            DateTime birthDate)
        {
            var principal = new ClaimsPrincipal();

            var identity = new ClaimsIdentity();

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, givenName));
            identity.AddClaim(new Claim(ClaimTypes.Surname, surname));
            identity.AddClaim(new Claim(ClaimTypes.DateOfBirth, birthDate.ToString()));

            principal.AddIdentity(identity);

            return principal;
        }


    }
}
