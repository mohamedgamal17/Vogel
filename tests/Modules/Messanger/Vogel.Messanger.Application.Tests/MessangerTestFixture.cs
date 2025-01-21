using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Respawn.Graph;
using Vogel.Application.Tests;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Messanger.Application.Tests.Fakers;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.Infrastructure.EntityFramework;
using Vogel.Messanger.MongoEntities.Messages;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Messanger.Application.Tests
{
    [TestFixture]
    public class MessangerTestFixture : TestFixture
    {
        public FakeUserService UserService { get; }

        public MessangerTestFixture()
        {
            UserService = ServiceProvider.GetRequiredService<FakeUserService>();
        }
        protected override Task SetupAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.InstallModule<MessangerApplicationTestModuleInstaller>(configuration, hostEnvironment);
            return Task.CompletedTask;
        }
        protected override async Task InitializeAsync(IServiceProvider services)
        {
            ResetInMemoryUsers(services);
            await ResetSqlDb(services);
            await DropMongoDb(services);

            await services.RunModulesBootstrapperAsync();
            await SeedData(services);
        }

        protected async Task SeedData(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<MessangerDbContext>();
            var userService = services.GetRequiredService<FakeUserService>();
            var userFriendService = services.GetRequiredService<FakeUserFriendService>();
            var messageMongoRepository = services.GetRequiredService<MessageMongoRepository>();
            var users = await SeedUsers(userService);
            await SeedUsersFriends(userFriendService, users);
            var conversationTable = await SeedConversations(dbContext, userFriendService, users);
            await SeedMessages(dbContext, messageMongoRepository, conversationTable);
        }

        private Task<List<UserDto>> SeedUsers(FakeUserService userService)
        {

            var users = new UserFaker().Generate(15);

            userService.AddRangeOfUsers(users);

            return Task.FromResult(users);
        }

        private Task SeedUsersFriends(FakeUserFriendService friendService, List<UserDto> users)
        {

            foreach (var user in users)
            {
                var friends = users.Where(x => x.Id != user.Id).PickRandom(5).ToList();

                friendService.AddRangeOfFriens(user, friends);

            }

            return Task.CompletedTask;
        }

        private async Task<Dictionary<string, List<Participant>>> SeedConversations(MessangerDbContext dbContext, FakeUserFriendService friendService , List<UserDto> users)
        {
            var faker = new Faker();

            Dictionary<string, List<Participant>> conversationTable = new ();

            foreach (var user in users)
            {
                var friends = friendService.GetUserFriends(user.Id);

                foreach (var friend in friends)
                {
                    bool hasName = faker.Random.Bool();

                    var conversation = new Conversation()
                    {
                        Name = hasName ? Guid.NewGuid().ToString() : null
                    };

                    var participants = new List<Participant>
                    {
                        new Participant()
                        {
                            ConversationId = conversation.Id,
                            UserId = user.Id
                        },

                        new Participant()
                        {
                            ConversationId = conversation.Id,
                            UserId = friend.TargetId
                        },
                    };

                    await dbContext.AddAsync(conversation);
                    await dbContext.AddRangeAsync(participants);

                    conversationTable[conversation.Id] = participants;
                }
            }

            await dbContext.SaveChangesAsync();

            return conversationTable;
        }

        private async Task SeedMessages(MessangerDbContext dbContext ,MessageMongoRepository messageMongoRepository  ,Dictionary<string, List<Participant>> conversationTable)
        {
            var faker = new Faker();

            var allLogs = new List<MessageLog>();

            foreach (var kvp in conversationTable)
            {
                List<MessageLog> conversationMessageLogs = new List<MessageLog>();

                var messages = Enumerable.Range(0, 10)
                    .Select(_ => new Message
                    {
                        ConversationId = kvp.Key,
                        SenderId = kvp.Value.PickRandom()!.Id,
                        Content = Guid.NewGuid().ToString(),
                    }).ToList();

                
                foreach(var message in messages)
                {
                    bool hasLog = faker.Random.Bool();

                    if (hasLog)
                    {
                        var log = new MessageLog
                        {
                            MessageId = message.Id,
                            SeenAt = DateTime.UtcNow,
                            SeenById = conversationTable[message.ConversationId].Where(x => x.UserId != message.SenderId).First().UserId,
                        };

                        conversationMessageLogs.Add(log);
                    }
                }

                await dbContext.AddRangeAsync(messages);
                await dbContext.AddRangeAsync(conversationMessageLogs);

                allLogs.AddRange(conversationMessageLogs);
            }


            await dbContext.SaveChangesAsync();

            await InsertMessageLogsMongoEntity(messageMongoRepository, allLogs);

            async Task InsertMessageLogsMongoEntity(MessageMongoRepository mongoRepository , List<MessageLog> messages)
            {
                foreach (var log in messages)
                {
                    var logMongoEntity = new MessageLogMongoEntity
                    {
                        Id = log.Id,
                        SeenAt = log.SeenAt,
                        SeenById = log.SeenById,
                        CreationTime = log.CreationTime,
                        CreatorId = log.CreatorId,
                        ModifierId = log.ModifierId,
                        ModificationTime = log.ModificationTime,
                        DeletorId = log.DeletorId,
                        DeletionTime = log.DeletionTime
                    };

                    var updateDefination = Builders<MessageMongoEntity>
                        .Update
                        .Set(x => x.Logs, new List<MessageLogMongoEntity> { logMongoEntity });

                    await mongoRepository.UpdateAsync(log.MessageId, updateDefination);
                }
                
            }
        }
        protected override async Task ShutdownAsync(IServiceProvider services)
        {
            ResetInMemoryUsers(services);
            await ResetSqlDb(services);
            await DropMongoDb(services);
        }

        protected async Task ResetSqlDb(IServiceProvider services)
        {
            var config = services.GetRequiredService<IConfiguration>();

            var respwan = await Respawn.Respawner.CreateAsync(config.GetConnectionString("Default")!, new Respawn.RespawnerOptions
            {
                TablesToIgnore = new Table[]
                {
                  "sysdiagrams",
                  "tblUser",
                  "tblObjectType",
                  "__EFMigrationsHistory"
                },
                SchemasToInclude = new string[]
                {
                    "Messanger"
                }

            });

            await respwan.ResetAsync(config.GetConnectionString("Default")!);
        }
        protected void ResetInMemoryUsers(IServiceProvider services)
        {
            var userService = services.GetRequiredService<FakeUserService>();
            var userFriendService = services.GetRequiredService<FakeUserFriendService>();
            userService.Reset();
            userFriendService.Reset();
        }

    }
}
