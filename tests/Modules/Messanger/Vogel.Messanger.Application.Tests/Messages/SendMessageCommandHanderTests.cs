using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.MongoEntities.Messages;
using Vogel.Messanger.Domain.Messages;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Messanger.Domain;
using Vogel.Application.Tests.Extensions;
using FluentAssertions;
using Vogel.Messanger.Application.Tests.Extensions;
using Vogel.Messanger.Application.Messages.Commands.SendMessage;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Users;
using Vogel.BuildingBlocks.Domain.Exceptions;
namespace Vogel.Messanger.Application.Tests.Messages
{
    public class SendMessageCommandHanderTests : MessangerTestFixture
    {
        public IMessangerRepository<Message> MessageRepository { get; private set; }
        public IMongoRepository<MessageMongoEntity> MessageMongoRepository { get; private set; }
        public IMessangerRepository<Conversation> ConversationRepository { get; private set; }
        public IMessangerRepository<Participant> ParticipantRepository { get; private set; }
        public IMongoRepository<UserMongoEntity> UserMongoRepository { get; private set; }


        public SendMessageCommandHanderTests()
        {
            MessageRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Message>>();
            MessageMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<MessageMongoEntity>>();
            ConversationRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Conversation>>();
            ParticipantRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Participant>>();
            UserMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<UserMongoEntity>>();
        }

        [Test]
        public async Task Should_send_message_to_other_user()
        {
            UserService.Login();

            var currentUser = await CreateFakeUserAsync(UserService.GetCurrentUser()!.Id);

            var antotherUser = await CreateFakeUserAsync(Guid.NewGuid().ToString());

            var conversation = await CreateFakeConversationAsync(new List<string> { currentUser.Id, antotherUser.Id });

            var command = new SendMessageCommand
            {
                ConversationId = conversation.Id,
                Content = Guid.NewGuid().ToString(),
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var message = await MessageRepository.FindByIdAsync(result.Value!.Id);

            message.Should().NotBeNull();

            message!.AssertSendMessageCommand(command, UserService.GetCurrentUser()!.Id);

            var mongoEntity = await MessageMongoRepository.FindByIdAsync(result.Value!.Id);

            mongoEntity.Should().NotBeNull();

            message!.AssertMessageMongoEntity(mongoEntity!);

            message!.AssertMessageDto(result.Value);
        }

        [Test]
        public async Task Should_failure_while_sending_message_when_user_is_not_authorized()
        {
            var command = new SendMessageCommand
            {
                Content = Guid.NewGuid().ToString(),
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_while_sending_message_when_conversation_is_not_exist()
        {
            UserService.Login();

            var command = new SendMessageCommand
            {
                ConversationId = Guid.NewGuid().ToString(),
                Content = Guid.NewGuid().ToString(),
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_sending_message_when_user_is_not_participant_in_conversation()
        {
            UserService.Login();

            List<string> participants = new List<string>
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };

            var conversation = await CreateFakeConversationAsync(participants);

            var command = new SendMessageCommand
            {
                Content = Guid.NewGuid().ToString(),
                ConversationId = conversation.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        private async Task<UserMongoEntity> CreateFakeUserAsync(string userId)
        {
            var user = new UserMongoEntity
            {
                Id = userId,
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Gender = Social.Shared.Common.Gender.Male,
                BirthDate = DateTime.Now.AddYears(-18),

            };

            await UserMongoRepository.ReplaceOrInsertAsync(user);

            return user;
        }

        private async Task<Conversation> CreateFakeConversationAsync(List<string> participants)
        {
            var conversation = new Conversation
            {
                Name = Guid.NewGuid().ToString()
            };

            await ConversationRepository.InsertAsync(conversation);

            var participantsEntities = participants.Select(userId => new Participant
            {
                ConversationId = conversation.Id,
                UserId = userId
            }).ToList();

            await ParticipantRepository.InsertManyAsync(participantsEntities);

            return conversation;
        }

    }
}
