using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.Application.Messages.Commands.MarkMessageAsSeen;
using Vogel.Messanger.Application.Tests.Extensions;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.MongoEntities.Messages;
namespace Vogel.Messanger.Application.Tests.Commands
{
    public class MarkMessageAsSeenCommandHandlerTests : MessangerTestFixture
    {
        public IMessangerRepository<Message> MessageRepository { get; set; }
        public IMongoRepository<MessageMongoEntity> MessageMongoRepository { get; set; }

        public MarkMessageAsSeenCommandHandlerTests()
        {
            MessageRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Message>>();
            MessageMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<MessageMongoEntity>>();
        }

        [Test]
        public async Task Should_mark_message_as_seen()
        {
            UserService.Login();

            var reciverId = UserService.GetCurrentUser()!.Id;

            var senderId = Guid.NewGuid().ToString();

            var fakeMessage = await CreateFakeMessage(senderId, reciverId);

            var command = new MarkMessageAsSeenCommand
            {
                MessageId = fakeMessage.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var message = await MessageRepository.FindByIdAsync(fakeMessage.Id);

            message.Should().NotBeNull();

            message!.IsSeen.Should().BeTrue();

            var mongoEntity = await MessageMongoRepository.FindByIdAsync(fakeMessage.Id);

            mongoEntity.Should().NotBeNull();

            mongoEntity!.IsSeen.Should().BeTrue();

            message.AssertMessageDto(result.Value!);
        }

        [Test]
        public async Task Should_failure_while_marking_message_as_seen_when_user_is_not_authorized()
        {
            var reciverId = Guid.NewGuid().ToString();

            var senderId = Guid.NewGuid().ToString();

            var fakeMessage = await CreateFakeMessage(senderId, reciverId);

            var command = new MarkMessageAsSeenCommand
            {
                MessageId = fakeMessage.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_while_marking_message_as_seen_when_user_is_not_the_reciver()
        {
            UserService.Login();

            var reciverId = Guid.NewGuid().ToString();

            var senderId = UserService.GetCurrentUser()!.Id;

            var fakeMessage = await CreateFakeMessage(senderId, reciverId);

            var command = new MarkMessageAsSeenCommand
            {
                MessageId = fakeMessage.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        [Test]
        public async Task Should_failure_while_marking_message_as_seen_when_message_is_already_marked()
        {
            UserService.Login();

            var reciverId = UserService.GetCurrentUser()!.Id;

            var senderId = Guid.NewGuid().ToString();

            var fakeMessage = await CreateFakeMessage(senderId, reciverId, true);

            var command = new MarkMessageAsSeenCommand
            {
                MessageId = fakeMessage.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(BusinessLogicException));
        }

        private async Task<Message> CreateFakeMessage(string senderId, string reciverId, bool isSeen = false)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReciverId = reciverId,
                Content = Guid.NewGuid().ToString()
            };

            if (isSeen)
            {
                message.MarkAsSeen();
            }

            return await MessageRepository.InsertAsync(message);
        }


    }
}
