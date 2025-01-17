﻿using FluentAssertions;
using Vogel.Application.Tests.Extensions;
using Vogel.Messanger.Application.Conversations.Commands.CreateConversation;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.Application.Tests.Extensions
{
    public static class ConversationAssertionExtensions
    {
        public static void AssertCreateConversationCommand(this Conversation conversation, CreateConversationCommand command , string currentUserId ,  List<Participant> participants)
        {
            conversation.Name.Should().Be(command.Name);

            var userIds =  participants.Select(x => x.UserId).Order();

            var commandUsers = command.Participants;

            commandUsers.Add(currentUserId);

            userIds.Should().BeEquivalentTo(commandUsers.Order());
        }

        public static void AssertConversationMongoEntity(this ConversationMongoEntity mongoEntity ,   Conversation conversation , List<Participant> participants)
        {
            mongoEntity.Id.Should().Be(conversation.Id);
            mongoEntity.Name.Should().Be(conversation.Name);
            mongoEntity.AssertAuditingProperties(conversation);

            var orderedParticipants = participants.OrderBy(x => x.Id).ToList();

            var orderedParticipantMongoEntities = mongoEntity.Participants.OrderBy(x => x.Id).ToList();

            orderedParticipants.Count.Should().Be(orderedParticipantMongoEntities.Count);

            for(int i = 0; i < orderedParticipants.Count; i++)
            {
                var participantMongoEntity = orderedParticipantMongoEntities[i];

                var participant = orderedParticipants[i];

                participantMongoEntity.AssertParticipantMongoEntity(participant);
            }
        }

        public static void AssertParticipantMongoEntity(this ParticipantMongoEntity mongoEntity , Participant participant)
        {
            mongoEntity.Id.Should().Be(participant.Id);
            mongoEntity.UserId.Should().Be(participant.UserId);
            mongoEntity.ConversationId.Should().Be(participant.ConversationId);
            mongoEntity.AssertAuditingProperties(participant);
        }

        public static void AssertParticipantDto(this ParticipantDto dto , Participant participant)
        {
            dto.Id.Should().Be(participant.Id);
            dto.UserId.Should().Be(participant.UserId);
            dto.ConversationId.Should().Be(participant.ConversationId);
        }
    }
}
