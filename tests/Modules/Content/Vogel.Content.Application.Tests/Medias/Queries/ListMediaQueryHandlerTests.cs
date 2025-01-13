using FluentAssertions;
using Vogel.Application.Tests.Extensions;
using Vogel.Content.Application.Medias.Queries.ListMedia;
namespace Vogel.Content.Application.Tests.Medias.Queries
{
    public class ListMediaQueryHandlerTests : ContentTestFixture
    {

        [Test]
        public async Task Should_list_current_user_medias()
        {
            var fakeUser = UserService.PickRandomUser()!;

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new ListMediaQuery
            {
                Limit = 20
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.All(x => x.UserId == fakeUser.Id).Should().BeTrue();
        }

        [Test]
        public async Task Should_failure_while_listing_current_user_media_when_user_is_not_authorized()
        {
            var query = new ListMediaQuery { Limit = 20 };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }

    }
}
