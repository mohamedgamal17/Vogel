using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.Social.Application.Users.Queries.ListUsersByIds;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Users;

namespace Vogel.Social.Application.Tests.Users.Queries
{
    public class ListUsersByIdsQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<User> UserRepository { get; }
        public ListUsersByIdsQueryHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
        }

        [Test]
        public async Task Should_list_users_by_ids()
        {
            UserService.Login();

            var randomUsers = await UserRepository.AsQuerable().PickRandom(5);

            var targetIds = randomUsers.Select(x => x.Id).Order().ToList();

            var query = new ListUsersByIdsQuery
            {
                Ids = targetIds,
                Limit = 5,
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.Select(x => x.Id).Order().Should().BeEquivalentTo(targetIds);
        }

        [Test]
        public async Task Should_failure_while_listing_users_by_ids_when_user_is_not_authorized()
        {
            var randomUsers = await UserRepository.AsQuerable().PickRandom(5);

            var targetIds = randomUsers.Select(x => x.Id).ToList();

            var query = new ListUsersByIdsQuery
            {
                Ids = targetIds,
                Limit = 5,
            };


            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

    }
}
