using FluentAssertions;
using Vogel.Application.Tests.Extensions;
using Vogel.MediaEngine.Application.Medias.Queries.ListMedia;

namespace Vogel.MediaEngine.Application.Tests.Medias.Queries
{
    public class ListMediaQueryHandlerTests : MediaEngineTestFixture
    {
        [Test]
        public async Task Should_list_current_user_medias()
        {
            AuthenticationService.Login("user-1", "user-1", new List<string>());
            var result = await Mediator.Send(new ListMediaQuery { Limit = 10 });
            result.ShouldBeSuccess();
            result.Value!.Data.Should().OnlyContain(x => x.UserId == "user-1");
        }
    }
}
