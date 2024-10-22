using FluentAssertions;
using Vogel.Application.Tests.Extensions;
using Vogel.Messanger.Application.Messages.Commands.SendMessage;
using Vogel.Messanger.Application.Messages.Dtos;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.MongoEntities.Messages;

namespace Vogel.Messanger.Application.Tests.Extensions
{
    public static class MessageAssertionExtensions
    {
        public static void AssertSendMessageCommand(this Message message , SendMessageCommand command , string userId)
        {
            message.Content.Should().Be(command.Content);
            message.SenderId.Should().Be(userId);
            message.IsSeen.Should().BeFalse();
        }

        public static void AssertMessageMongoEntity(this Message message , MessageMongoEntity mongoEntity)
        {
            mongoEntity.Id.Should().Be(message.Id);
            mongoEntity.Content.Should().Be(message.Content);
            mongoEntity.SenderId.Should().Be(message.SenderId);
            mongoEntity.AssertAuditingProperties(message);
        }
        public static void AssertMessageDto(this Message message, MessageDto dto)
        {
            dto.Id.Should().Be(message.Id);
            dto.Content.Should().Be(message.Content);
            dto.SenderId.Should().Be(message.SenderId);
            dto.IsSeen.Should().Be(message.IsSeen);
            dto.Sender.Should().NotBeNull();
            dto.Reciver.Should().NotBeNull();
        }
    }
}
