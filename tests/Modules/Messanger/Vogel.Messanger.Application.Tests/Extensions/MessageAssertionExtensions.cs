﻿using FluentAssertions;
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
            message.ConversationId.Should().Be(command.ConversationId);
            message.SenderId.Should().Be(userId);
        }

        public static void AssertMessageMongoEntity(this Message message , MessageMongoEntity mongoEntity)
        {
            mongoEntity.Id.Should().Be(message.Id);
            mongoEntity.ConversationId.Should().Be(message.ConversationId);
            mongoEntity.Content.Should().Be(message.Content);
            mongoEntity.SenderId.Should().Be(message.SenderId);
            mongoEntity.AssertAuditingProperties(message);
        }
        public static void AssertMessageDto(this Message message, MessageDto dto)
        {
            dto.Id.Should().Be(message.Id);
            dto.ConversationId.Should().Be(message.ConversationId);
            dto.Content.Should().Be(message.Content);
            dto.SenderId.Should().Be(message.SenderId);
            dto.Sender.Should().NotBeNull();
        }

        public static void AssertMessageActivityMongoEntity(this MessageActivityMongoEntity mongoEntity , MessageActivity messageActivity)
        {
            mongoEntity.Id.Should().Be(messageActivity.Id);
            mongoEntity.SeenById.Should().Be(messageActivity.SeenById);
            mongoEntity.SeenAt.Should().BeCloseTo(messageActivity.SeenAt, TimeSpan.FromMilliseconds(1));
            mongoEntity.MessageId.Should().Be(messageActivity.MessageId);
            mongoEntity.AssertAuditingProperties(messageActivity);
        }
    }
}
