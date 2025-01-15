using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Content.Application.PostReactions.Queries.ListPostReactions;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Posts;

namespace Vogel.Content.Application.Tests.PostReactions.Queries
{
    public class ListPostReactionsQueryHandlerTests : ContentTestFixture
    {
        public IContentRepository<Post> PostRepository { get; }

        public ListPostReactionsQueryHandlerTests()
        {
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
        }

        [Test]
        public async Task Should_get_post_all_reactions()
        {
            var fakePost = await PostRepository.AsQuerable().PickRandom();

            AuthenticationService.Login();

            var query = new ListPostReactionsQuery
            {
                PostId = fakePost!.Id,
                Limit = 10
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.All(x => x.PostId == fakePost.Id).Should().BeTrue();
        }

        [Test]
        public async Task Should_failure_while_getting_post_reactions_when_post_id_is_not_exist()
        {
            AuthenticationService.Login();

            var query = new ListPostReactionsQuery
            {
                PostId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_getting_post_reactions_when_user_is_not_authorized()
        {
            var query = new ListPostReactionsQuery
            {
                PostId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
    }
}
