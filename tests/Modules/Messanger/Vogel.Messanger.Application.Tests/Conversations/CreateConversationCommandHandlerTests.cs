using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Microsoft.EntityFrameworkCore;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.Application.Conversations.Commands.CreateConversation;
using Vogel.Messanger.Application.Tests.Extensions;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Conversations;
using MongoDB.Driver;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.Messanger.MongoEntities.Users;
namespace Vogel.Messanger.Application.Tests.Conversations
{
    public class CreateConversationCommandHandlerTests : MessangerTestFixture
    {
        public IMessangerRepository<Conversation> ConversationRepository { get;  }
        public IMessangerRepository<Participant> ParticipantRepository { get; }
        public IMongoRepository<ConversationMongoEntity> ConversationMongoRepository { get; }
        public IMongoRepository<ParticipantMongoEntity> ParticipantMongoRepository { get; }
        public IMongoRepository<UserMongoEntity> UserMongoRepository { get; }
        public CreateConversationCommandHandlerTests()
        {
            ConversationRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Conversation>>();
            ParticipantRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Participant>>();
            ConversationMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<ConversationMongoEntity>>();
            ParticipantMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<ParticipantMongoEntity>>();
            UserMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<UserMongoEntity>>();
        }

        [Test]
        public async Task Should_create_conversation()
        {
            UserService.Login();

            string currentUserId = UserService.GetCurrentUser()!.Id;

            await InsertUser(currentUserId);

            var userFriend = await InsertUser(Guid.NewGuid().ToString());

            var commmand = new CreateConversationCommand
            {
                Participants = new List<string>
                {
                    userFriend.Id
                }
            };

            var result = await Mediator.Send(commmand);

            result.ShouldBeSuccess();

            var conversation = await ConversationRepository.FindByIdAsync(result.Value!.Id);

            var participants = await ParticipantRepository.AsQuerable().Where(x => x.ConversationId == result.Value.Id).ToListAsync();

            conversation.Should().NotBeNull();

            participants.Count.Should().BeGreaterThan(1);

            conversation!.AssertCreateConversationCommand(commmand, currentUserId, participants);

            var conversationMongoEntity = await ConversationMongoRepository.FindByIdAsync(result.Value!.Id);

            var participantsMongoEntity = await ParticipantMongoRepository
                .ApplyFilterAsync(Builders<ParticipantMongoEntity>.Filter.Eq(x => x.ConversationId, result.Value!.Id));

            conversationMongoEntity.Should().NotBeNull();

            participantsMongoEntity.Count.Should().BeGreaterThan(1);

            conversationMongoEntity!.AssertConversationMongoEntity(participantsMongoEntity, conversation!, participants);

            result.Value.AssertConversationDto(conversation!, participants);
        }

        [Test] 
        public async Task Should_failure_while_creating_conversation_when_user_is_not_authorized()
        {
            var userFriend = Guid.NewGuid().ToString();

            var command = new CreateConversationCommand
            {
                Participants = new List<string>
                {
                    userFriend
                }
            };

            var results = await Mediator.Send(command);

            results.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }


        private async Task<UserMongoEntity> InsertUser(string userId)
        {
            var entity = new UserMongoEntity
            {
                Id = userId,
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Social.Shared.Common.Gender.Male,

            };

            return await UserMongoRepository.InsertAsync(entity);
        }
    }

   
}
