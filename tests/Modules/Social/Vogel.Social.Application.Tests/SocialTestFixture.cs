using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.Application.Tests;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Social.Application.Tests.Fakers;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.Domain.Users;
using Vogel.Social.Infrastructure.EntityFramework;

namespace Vogel.Social.Application.Tests
{
    [TestFixture]
    public class SocialTestFixture : TestFixture
    {
        protected override Task SetupAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.InstallModule<SocialApplicationTestModuleInstaller>(configuration,hostEnvironment);
            return Task.CompletedTask;
        }
        protected override async Task InitializeAsync(IServiceProvider services)
        {
            await services.RunModulesBootstrapperAsync();

            await SeedTestData(services);
        }

        private async Task SeedTestData(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<SocialDbContext>();

            var users = await SeedUsers(dbContext);

            await SeedMedias(dbContext, users);

            await SeedFriendRequest(dbContext, users);

            await SeedFriends(dbContext, users);

        }

        private async Task<List<User>> SeedUsers(SocialDbContext dbContext)
        {
            var users = new UserFaker().Generate(50);

            await dbContext.AddRangeAsync(users);

            await dbContext.SaveChangesAsync();

            return users;
        }

        private async Task SeedMedias(SocialDbContext dbContext , List<User> users)
        {
            var faker = new Faker();

            int mediaCount = 3;

            foreach (var user in users)
            {
                var medias = new MediaFaker(user).Generate(mediaCount);

                await dbContext.AddRangeAsync(medias);

                bool hasAvatar = faker.Random.Bool();

                if (hasAvatar)
                {
                    var avatar = faker.PickRandom(medias);

                    user.AvatarId = avatar.Id;
                }
            }

            await dbContext.SaveChangesAsync();
        }



        private async Task SeedFriendRequest (SocialDbContext dbContext, List<User> users)
        {
            var faker = new Faker();

            List<FriendRequest> requests = new List<FriendRequest>();

            foreach (var user in users)
            {
                var currentUserFriendRequests = requests.Where(x => x.ReciverId == user.Id).Select(x => x.SenderId).ToList();

                int randomPick = users.Count - (1 + currentUserFriendRequests.Count) > 5 ? 5 :
                    users.Count - (1 + currentUserFriendRequests.Count);

                if(randomPick > 0)
                {
                    var randomUserFriends = faker.PickRandom(users.Where(x => x.Id != user.Id && !currentUserFriendRequests.Contains(x.Id)), randomPick);


                    var userFriendRequests = randomUserFriends?.Select(x => new FriendRequest
                    {
                        SenderId = x.Id,
                        ReciverId = user.Id
                    });

                    await dbContext.AddRangeAsync(userFriendRequests!);

                    requests.AddRange(userFriendRequests!);
                }
          
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task SeedFriends(SocialDbContext dbContext  , List<User> users)
        {
            var faker = new Faker();

            List<Friend> friends = new List<Friend>();

            foreach (var user in users)
            {
                var currentUserFriends = friends.Where(x => x.TargetId == user.Id).Select(x => x.SourceId).ToList();


                int randomPick = users.Count - (1 + currentUserFriends.Count) > 5 ? 5 :
                    users.Count - (1 + currentUserFriends.Count);

                if(randomPick > 0)
                {
                    var randomUserFriends = faker.PickRandom(users.Where(x => x.Id != user.Id && !currentUserFriends.Contains(x.Id)), randomPick);


                    var userFriends = randomUserFriends?.Select(x => new Friend
                    {
                        SourceId = user.Id,
                        TargetId = x.Id
                    });

                    await dbContext.AddRangeAsync(userFriends!);

                    friends.AddRange(userFriends!);
                }           
            }

            await dbContext.SaveChangesAsync();
        }


        protected override async Task ShutdownAsync(IServiceProvider services)
        {
            await DropSqlDb();
            await DropMongoDb();
        }
    }
}
