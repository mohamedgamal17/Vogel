using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Content.Application.Comments.Queries.ListComments;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Posts;
namespace Vogel.Content.Application.Tests.Comments.Queries
{
    public class ListCommentsQueryHandlerTests : ContentTestFixture
    {
        public IContentRepository<Post> PostRepository { get; }

        public ListCommentsQueryHandlerTests()
        {
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
        }

        [Test]
        public async Task Should_list_comments()
        {
            var fakeUser = UserService.PickRandomUser()!;

            var fakePost = await PostRepository.AsQuerable().PickRandom();

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new ListCommentsQuery
            {
                PostId = fakePost!.Id,
                Limit = 10
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.All(x => x.PostId == fakePost.Id).Should().BeTrue();
        }

        [Test]
        public async Task Should_failure_while_listing_comments_when_post_id_is_not_exist()
        {
            AuthenticationService.Login();

            var query = new ListCommentsQuery
            {
                PostId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_listing_comments_when_user_is_not_authorized()
        {
            var query = new ListCommentsQuery
            {
                PostId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }     
    }
}
