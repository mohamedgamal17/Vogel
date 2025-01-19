using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Messanger.Application.Conversations.Queries.ListUserConversationParticipant;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Domain.Conversations;

namespace Vogel.Messanger.Application.Tests.Conversations.Queries
{
    public class ListUserConversationParticipantQueryHandlerTests  : MessangerTestFixture
    {

        public IMessangerRepository<Conversation> ConversationRepository { get; }
        public IMessangerRepository<Participant> ParticipantRepository { get; }

        public ListUserConversationParticipantQueryHandlerTests()
        {
            ConversationRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Conversation>>();
            ParticipantRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Participant>>();
        }

        [Test]
        public async Task Should_list_conversation_participants()
        {
            var fakeUser = UserService.PickRandomUser();
            var fakeConversation = await PrepareUserConversation(fakeUser!.Id).PickRandom();
            var expectedParticipantCount = await ParticipantRepository.AsQuerable().Where(x => x.ConversationId == fakeConversation!.Id).CountAsync();
            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());


            var query = new ListUserConversationParticipantQuery
            {
                ConversationId = fakeConversation!.Id,
                Limit = 50,
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.Count.Should().Be(expectedParticipantCount);

            result.Value.Data.All(x => x.ConversationId == fakeConversation.Id).Should().BeTrue();
        }

        [Test]
        public async Task Should_failure_while_listing_conversation_participants_when_conversation_id_is_not_exist()
        {
            var fakeUser = UserService.PickRandomUser()!;
            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new ListUserConversationParticipantQuery
            {
                ConversationId = Guid.NewGuid().ToString(),
                Limit = 50
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_listing_conversation_participant_when_user_is_not_authorized()
        {
            var query = new ListUserConversationParticipantQuery
            {
                ConversationId = Guid.NewGuid().ToString(),
                Limit = 50
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }


        private  IQueryable<Conversation> PrepareUserConversation(string userId)
        {

            var query = from p in ParticipantRepository.AsQuerable()
                        where p.UserId == userId
                        join conv in ConversationRepository.AsQuerable()
                        on p.ConversationId equals conv.Id
                        select conv;


            return  query;
        }
    }
}
