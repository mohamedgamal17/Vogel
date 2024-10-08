using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.MongoEntities.Messages;
using Vogel.Messanger.Domain.Messages;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Application.Messages.Commands;
using Vogel.Application.Tests.Extensions;
using FluentAssertions;
using Vogel.Messanger.Application.Tests.Extensions;
namespace Vogel.Messanger.Application.Tests.Commands
{
    public class SendMessageCommandHanderTests : MessangerTestFixture
    {
        public IMessangerRepository<Message> MessageRepository { get;private set; }
        public IMongoRepository<MessageMongoEntity> MessageMongoRepository { get; private set; }

        public SendMessageCommandHanderTests()
        {
            MessageRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Message>>();
            MessageMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<MessageMongoEntity>>();
        }

        [Test]
        public async Task Should_send_message_to_other_user()
        {
            UserService.Login();

            var command = new SendMessageCommand
            {
                Content = Guid.NewGuid().ToString(),
                ReciverId = UserService.GetCurrentUser()!.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var message = await  MessageRepository.FindByIdAsync(result.Value!.Id);

            message.Should().NotBeNull();

            message!.AssertSendMessageCommand(command, UserService.GetCurrentUser()!.Id);

            var mongoEntity = await  MessageMongoRepository.FindByIdAsync(result.Value!.Id);

            mongoEntity.Should().BeNull();

            message!.AssertMessageMongoEntity(mongoEntity!);

            message!.AssertMessageDto(result.Value);
        }

        [Test]
        public async Task Should_failure_while_sending_message_when_user_is_not_authorized()
        {
            var command = new SendMessageCommand
            {
                Content = Guid.NewGuid().ToString(),
                ReciverId = UserService.GetCurrentUser()!.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
    }
}
