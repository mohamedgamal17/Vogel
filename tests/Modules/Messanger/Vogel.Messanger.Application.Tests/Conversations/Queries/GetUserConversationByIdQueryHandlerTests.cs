using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Messanger.Application.Conversations.Queries.GetUserConversationById;
using Vogel.Messanger.Application.Tests.Extensions;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Domain.Conversations;
namespace Vogel.Messanger.Application.Tests.Conversations.Queries
{
    public class GetUserConversationByIdQueryHandlerTests : MessangerTestFixture
    {
        public IMessangerRepository<Conversation> ConversationRepository { get; }

        public IMessangerRepository<Participant> ParticipantRepository { get; }

        public GetUserConversationByIdQueryHandlerTests()
        {
            ConversationRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Conversation>>();
            ParticipantRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Participant>>();
        }

        [Test]
        public async Task Should_get_user_conversation_by_id()
        {
            var fakeUser = UserService.PickRandomUser()!;
            var fakeConversation = await GetRandomConversation(fakeUser.Id);
            var conversationParticipant = await ParticipantRepository.AsQuerable().Where(x => x.ConversationId == fakeConversation.Id).ToListAsync();

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new GetConversationByIdQuery
            {
                ConversationId = fakeConversation.Id
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertConversationDto(fakeConversation, conversationParticipant);         
        }

 

        [Test]
        public async Task Should_failure_while_getting_user_conversation_by_id_when_conversation_id_is_not_exist()
        {
            var fakeUser = UserService.PickRandomUser()!;

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new GetConversationByIdQuery
            {
                ConversationId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_getting_user_conversation_by_id_when_user_is_not_authorized()
        {
            var fakeConversation = await ConversationRepository.AsQuerable().PickRandom();

            var query = new GetConversationByIdQuery
            {
                ConversationId = fakeConversation!.Id
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
