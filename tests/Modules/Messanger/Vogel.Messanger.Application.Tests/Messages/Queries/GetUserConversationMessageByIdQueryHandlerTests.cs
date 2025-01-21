using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Messanger.Application.Messages.Dtos;
using Vogel.Messanger.Application.Messages.Queries.GetUserConversationMessage;
using Vogel.Messanger.Application.Tests.Extensions;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain.Messages;

namespace Vogel.Messanger.Application.Tests.Messages.Queries
{
    public class GetUserConversationMessageByIdQueryHandlerTests : MessangerTestFixture
    {
        public IMessangerRepository<Conversation> ConversationRepository { get; set; }
        public IMessangerRepository<Participant> ParticipantRepository { get; set; }
        public IMessangerRepository<Message> MessageRepository { get; set; }
        public IMessangerRepository<MessageLog> MessageLogRepository { get; set; }

        public GetUserConversationMessageByIdQueryHandlerTests()
        {
            ConversationRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Conversation>>();
            ParticipantRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Participant>>();
            MessageRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Message>>();
            MessageLogRepository = ServiceProvider.GetRequiredService<IMessangerRepository<MessageLog>>();
        }


        [Test]
        public async Task Should_get_user_conversation_message_by_id()
        {
            var fakeUser =  UserService.PickRandomUser()!;
            var fakeConversation = await GetRandomConversation(fakeUser.Id);
            var totalParticipant = await ParticipantRepository.AsQuerable().Where(x => x.ConversationId == fakeConversation.Id).CountAsync();
            var fakeMessage = await MessageRepository.AsQuerable().Where(x => x.ConversationId == fakeConversation.Id).PickRandom();
            var messageLogCount = await MessageLogRepository.AsQuerable().Where(x => x.MessageId == fakeMessage!.Id).CountAsync();

            bool expectedSeenMessage = messageLogCount == (totalParticipant - 1);

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new GetUserConversationMessageQuery
            {
                ConversationId = fakeConversation.Id,
                MessageId = fakeMessage!.Id
            };       

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertMessageDto(fakeMessage);

            result.Value!.IsSeen.Should().Be(expectedSeenMessage);
        }

        [Test]
        public async Task Should_failure_while_getting_user_conversation_message_by_id_when_conversation_is_not_exist()
        {
            var fakeUser = UserService.PickRandomUser()!;

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new GetUserConversationMessageQuery
            {
                ConversationId = Guid.NewGuid().ToString(),
                MessageId = Guid.NewGuid().ToString(),
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));

        }

        [Test]
        public async Task Should_failure_while_getting_user_conversation_message_by_id_when_message_id_is_not_exist()
        {
            var fakeUser = UserService.PickRandomUser()!;
            var fakeConversation = await GetRandomConversation(fakeUser.Id);
            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new GetUserConversationMessageQuery
            {
                ConversationId = fakeConversation.Id,
                MessageId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_getting_user_conversation_message_by_id_when_user_is_not_participant_in_conversation()
        {
            var fakeConversation = await ConversationRepository.AsQuerable().PickRandom();
            var fakeMessage = await MessageRepository.AsQuerable().Where(x => x.ConversationId == fakeConversation!.Id).PickRandom();

            AuthenticationService.Login();

            var query = new GetUserConversationMessageQuery
            {
                ConversationId = fakeConversation!.Id,
                MessageId = fakeMessage!.Id
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));

        }

        [Test]
        public async Task Should_failure_while_getting_conversation_message_by_id_when_user_is_not_authorized()
        {
            var fakeConversation = await ConversationRepository.AsQuerable().PickRandom();
            var fakeMessage = await MessageRepository.AsQuerable().Where(x => x.ConversationId == fakeConversation!.Id).PickRandom();


            var query = new GetUserConversationMessageQuery
            {
                ConversationId = fakeConversation!.Id,
                MessageId = fakeMessage!.Id
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        public async Task<Conversation> GetRandomConversation(string userId)
        {
            var query = from p in ParticipantRepository.AsQuerable()
                        where p.UserId == userId
                        join conv in ConversationRepository.AsQuerable()
                        on p.ConversationId equals conv.Id
                        select conv;


            return (await query.PickRandom())!;
        }
    }
}
