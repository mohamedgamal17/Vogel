using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.Messanger.Application.Conversations.Queries.GetUserConversationParticipant;
using Vogel.Messanger.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;

namespace Vogel.Messanger.Application.Tests.Conversations.Queries
{
    public class GetUserConversationParticipantByIdQueryHandlerTests : MessangerTestFixture
    {
        public IMessangerRepository<Conversation> ConversationRepository { get; }

        public IMessangerRepository<Participant> ParticipantRepository { get; }


        public GetUserConversationParticipantByIdQueryHandlerTests()
        {
            ConversationRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Conversation>>();
            ParticipantRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Participant>>();
        }

        [Test]
        public async Task Should_get_user_conversation_participant_by_id()
        {
            var fakeUser = UserService.PickRandomUser()!;
            var fakeConversation = await PrepareUserConversation(fakeUser.Id).PickRandom();
            var fakeParticipant = await ParticipantRepository.AsQuerable().Where(x => x.ConversationId == fakeConversation!.Id).PickRandom();
            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new GetUserConversationParticipantQuery
            {
                ConversationId = fakeConversation!.Id,
                ParticipantId = fakeParticipant!.Id
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertParticipantDto(fakeParticipant);
        }

        [Test]
        public async Task Should_failure_while_getting_user_conversation_participant_by_id_when_conversation_id_is_not_exist()
        {
            var fakeUser = UserService.PickRandomUser()!;
            var fakeParticipant = await ParticipantRepository.AsQuerable().PickRandom();

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new GetUserConversationParticipantQuery
            {
                ConversationId = Guid.NewGuid().ToString(),
                ParticipantId = fakeParticipant!.Id
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_getting_user_conversation_participant_by_id_when_participant_id_is_not_exist()
        {
            var fakeUser = UserService.PickRandomUser();
            var fakeConversation = await PrepareUserConversation(fakeUser!.Id).PickRandom();

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new GetUserConversationParticipantQuery
            {
                ConversationId = fakeConversation!.Id,
                ParticipantId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_getting_user_conversation_by_id_when_user_is_not_authorized()
        {

            var query = new GetUserConversationParticipantQuery
            {
                ConversationId = Guid.NewGuid().ToString(),
                ParticipantId = Guid.NewGuid().ToString(),
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
        private IQueryable<Conversation> PrepareUserConversation(string userId)
        {

            var query = from p in ParticipantRepository.AsQuerable()
                        where p.UserId == userId
                        join conv in ConversationRepository.AsQuerable()
                        on p.ConversationId equals conv.Id
                        select conv;


            return query;
        }
    }
}
