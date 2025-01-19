using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.Messanger.Application.Conversations.Queries.ListUserConversation;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Domain.Conversations;
namespace Vogel.Messanger.Application.Tests.Conversations.Queries
{
    public class ListUserConversationQueryHandlerTests : MessangerTestFixture
    {
        public IMessangerRepository<Conversation> ConversationRepository { get;  }

        public IMessangerRepository<Participant> ParticipantRepository { get; }

        public ListUserConversationQueryHandlerTests()
        {
            ConversationRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Conversation>>();
            ParticipantRepository = ServiceProvider.GetRequiredService<IMessangerRepository<Participant>>();
        }

        [Test]
        public async Task Should_get_current_user_conversations()
        {
            var fakeUser = UserService.PickRandomUser()!;
            var expectedConversations = await GetUserConversations(fakeUser.Id);

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new ListUserConversationQuery { Limit = 100 };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.Count.Should().Be(expectedConversations.Count);

            result.Value.Data.All(x => expectedConversations.Any(c => c.Id == x.Id)).Should().BeTrue();
        }


        [Test]
        public async Task Should_failure_while_getting_current_user_conversations_when_user_is_not_authorized()
        {
            var query = new ListUserConversationQuery { Limit = 100 };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }

        private async Task<List<Conversation>> GetUserConversations(string userId)
        {

            var query = from p in ParticipantRepository.AsQuerable()
                        where p.UserId == userId
                        join conv in ConversationRepository.AsQuerable()
                        on p.ConversationId equals conv.Id
                        select conv;


            return await query.ToListAsync();    
        }
    }
}
