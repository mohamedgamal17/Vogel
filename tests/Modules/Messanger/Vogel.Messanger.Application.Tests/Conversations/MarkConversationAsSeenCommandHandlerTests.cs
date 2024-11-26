using AutoMapper;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.Application.Conversations.Commands.MarkConversationAsSeen;
using Vogel.Messanger.Application.Messages.Events;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.MongoEntities.Messages;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using Vogel.Messanger.Application.Tests.Extensions;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
namespace Vogel.Messanger.Application.Tests.Conversations
{
    public class MarkConversationAsSeenCommandHandlerTests : MessangerTestFixture
    {
        public IRepository<Conversation> ConversationRepository { get; set; }
        public IRepository<Participant> ParticipantRepository { get; set; }
        public IRepository<Message> MessageRepository { get; set; }
        public IRepository<MessageActivity> MessageActivityRepository { get; set; }
        public IMongoRepository<MessageLogMongoEntity> MessageActivityMongoRepository { get; set; }
        public IMapper Mapper { get; set; }
        public ITestHarness TestHarness { get; }
        public MarkConversationAsSeenCommandHandlerTests()
        {
            ConversationRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Conversation>>();
            ParticipantRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Participant>>();
            MessageRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Message>>();
            MessageActivityRepository = ServiceProvider.GetRequiredService<IMessangerRepository<MessageActivity>>();
            MessageActivityMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<MessageLogMongoEntity>>();
            Mapper = ServiceProvider.GetRequiredService<IMapper>();
            TestHarness = ServiceProvider.GetRequiredService<ITestHarness>();
        }


        [Test]
        public async Task Should_mark_conversation_messages_as_seen()
        {
            await TestHarness.Start();

            string currentUser = Guid.NewGuid().ToString();
            string anotherUser = Guid.NewGuid().ToString();
            int numberOfGeneratedMessage = 500;
            int batchSize = 100;

            int numberOfPublishedEvents = (int)Math.Ceiling((double)(numberOfGeneratedMessage / batchSize));
            var conversation = await CreateFakeConversation(currentUser, anotherUser);

            var seenMessages = await GenerateFakeMessages(conversation.Id, anotherUser, numberOfGeneratedMessage);
            var unSeenMessages = await GenerateFakeMessages(conversation.Id, anotherUser, numberOfGeneratedMessage);

            List<string> unSeenMessagesIds = unSeenMessages.Select(x => x.Id).ToList();

            await MarkMessagesAssSeen(seenMessages, currentUser);

            var command = new MarkConversationAsSeenCommand
            {
                ConversationId = conversation.Id,
                UserId = currentUser
            };

            var result = await Mediator.Send(command);

            await TestHarness.Published.Any<LogMessagesActivitiesEvent>();

            var published =  TestHarness.Published.Select<LogMessagesActivitiesEvent>();

            published.Count().Should().Be(numberOfPublishedEvents);

            await TestHarness.Consumed.Any<LogMessagesActivitiesEvent>();

            var consumed = TestHarness.Consumed.Select<LogMessagesActivitiesEvent>();

             consumed.Count().Should().Be(numberOfPublishedEvents);

            result.IsSuccess.Should().BeTrue();

            var activites = await MessageActivityRepository.AsQuerable()
                .Where(x => unSeenMessagesIds.Contains(x.MessageId))
                .OrderBy(x => x.Id)
                .ToListAsync();

            var mongoActivites =  MessageActivityMongoRepository.AsQuerable()
                .Where(x => unSeenMessagesIds.Contains(x.MessageId))
                .OrderBy(x=> x.Id)
                .ToList();

            activites.Count.Should().Be(unSeenMessagesIds.Count);

            activites.Select(x => x.MessageId).Should().BeEquivalentTo(unSeenMessagesIds);

            mongoActivites.Count.Should().Be(activites.Count);

            for (int i = 0; i < activites.Count; i++)
            {
                mongoActivites[i].AssertMessageActivityMongoEntity(activites[i]);
            }

            await TestHarness.Stop();
        }

        [Test]
        public async Task Should_failure_while_marking_conversation_as_seen_when_conversation_is_not_exist()
        {
            string currentUser = Guid.NewGuid().ToString();

            var command = new MarkConversationAsSeenCommand
            {
                ConversationId = Guid.NewGuid().ToString(),
                UserId = currentUser
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }


        [Test]
        public async Task Should_failure_while_marking_conversation_as_seen_when_user_is_not_participant_in_the_conversation()
        {
            string currentUser = Guid.NewGuid().ToString();
             
            var conversation = await CreateFakeConversation(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var command = new MarkConversationAsSeenCommand
            {
                ConversationId = conversation.Id,
                UserId = currentUser
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }


        private async Task<List<MessageActivity>> MarkMessagesAssSeen(List<Message> messages, string seenbyId)
        {
            var activites = messages.Select(x => new MessageActivity
            {
                MessageId = x.Id,
                SeenById = seenbyId,
                SeenAt = DateTime.UtcNow
            }).ToList();

            await MessageActivityRepository.InsertManyAsync(activites);

            var mongoActivites = Mapper.Map<List< MessageActivity> ,List<MessageLogMongoEntity>>(activites);

            await MessageActivityMongoRepository.ReplaceOrInsertManyAsync(mongoActivites);

            return activites;
        }
        private async Task<List<Message>> GenerateFakeMessages(string conversationId , string senderId, int numberOfMessages = 10)
        {
            List<Message> messages = new List<Message>();
            
            for(int i =0; i < numberOfMessages; i++)
            {
                var msg = new Message
                {
                    Content = Guid.NewGuid().ToString(),
                    ConversationId = conversationId,
                    SenderId = senderId
                };

                messages.Add(msg);
            }

            return await MessageRepository.InsertManyAsync(messages);
        }

        private async Task<Conversation> CreateFakeConversation(string userId1, string userId2)
        {
            var conversation = new Conversation
            {
                Name = Guid.NewGuid().ToString()
            };

            await ConversationRepository.InsertAsync(conversation);

            List<Participant> participants = new List<Participant>
            {
                new Participant{ UserId = userId1 , ConversationId = conversation.Id},
                new Participant {UserId = userId2 , ConversationId = conversation.Id}
            };

            await ParticipantRepository.InsertManyAsync(participants);

            return conversation;
         }
    }
}
