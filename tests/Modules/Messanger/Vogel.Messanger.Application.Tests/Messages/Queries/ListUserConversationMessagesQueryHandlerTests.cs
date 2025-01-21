using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Messanger.Application.Messages.Queries.GetUserConversationMessage;
using Vogel.Messanger.Application.Messages.Queries.ListUserConversationMessage;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain.Messages;

namespace Vogel.Messanger.Application.Tests.Messages.Queries
{
    public class ListUserConversationMessagesQueryHandlerTests : MessangerTestFixture
    {
        public IMessangerRepository<Conversation> ConversationRepository { get; set; }
        public IMessangerRepository<Participant> ParticipantRepository { get; set; }

        public IMessangerRepository<Message> MessageRepository { get; set; }

        public ListUserConversationMessagesQueryHandlerTests()
        {
            ConversationRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Conversation>>();
            ParticipantRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Participant>>();
            MessageRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Message>>();
        }


        [Test]
        public async Task Should_list_user_conversation_messages()
        {
            var fakeUser = UserService.PickRandomUser()!;

            var fakeConversation = await GetRandomConversation(fakeUser!.Id);

            var expectedMessageCount = await MessageRepository.AsQuerable()
                .Where(x => x.ConversationId == fakeConversation.Id)
                .CountAsync();


            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new ListUserConversationMessageQuery
            {
                ConversationId = fakeConversation.Id,
                Limit = 100
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.All(x => x.ConversationId == fakeConversation.Id).Should().BeTrue();

            result.Value.Data.Count.Should().Be(expectedMessageCount);
        }

        [Test]
        public async Task Should_failure_while_listing_user_conversation_messages_when_conversation_id_is_not_exist()
        {
            var fakeUser = UserService.PickRandomUser()!;

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new ListUserConversationMessageQuery
            {
                ConversationId = Guid.NewGuid().ToString(),
                Limit = 10
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));

        }

        [Test]
        public async Task Should_failure_while_listing_user_conversation_messages_when_user_is_not_participant_in_conversation()
        {
            var fakeConversation = await ConversationRepository.AsQuerable().PickRandom();
            var fakeMessage = await MessageRepository.AsQuerable().Where(x => x.ConversationId == fakeConversation!.Id).PickRandom();

            AuthenticationService.Login();

            var query = new ListUserConversationMessageQuery
            {
                ConversationId = fakeConversation!.Id,
                Limit = 10
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));

        }

        [Test]
        public async Task Should_failure_while_listing_user_conversation_messages_when_user_is_not_authorized()
        {

            var query = new ListUserConversationMessageQuery
            {
                ConversationId = Guid.NewGuid().ToString(),
                Limit = 10
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
