using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Content.Application.PostReactions.Queries.GetPostReactionById;
using Vogel.Content.Application.Posts.Queries.GetPostbyId;
using Vogel.Content.Application.Tests.Extensions;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Posts;

namespace Vogel.Content.Application.Tests.PostReactions.Queries
{
    public class GetPostReactionByIdQueryHandlerTests : ContentTestFixture
    {
        public IContentRepository<Post> PostRepository { get;  }
        public IContentRepository<PostReaction> PostReactionRepository { get; }

        public GetPostReactionByIdQueryHandlerTests()
        {
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
            PostReactionRepository = ServiceProvider.GetRequiredService<IContentRepository<PostReaction>>();
        }

        [Test]
        public async Task Should_get_post_reaction_by_id()
        {
            var fakePost = await PostRepository.AsQuerable().PickRandom();

            var fakePostReaction = await PostReactionRepository.AsQuerable().Where(x => x.PostId == fakePost!.Id).PickRandom();

            AuthenticationService.Login();

            var query = new GetPostReactionByIdQuery
            {
                PostId = fakePost!.Id,
                PostReactionId = fakePostReaction!.Id
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertPostReactionDto(fakePostReaction);
        }

        [Test]
        public async Task Should_failure_while_getting_post_reaction_by_id_when_post_id_is_not_exist()
        {
            AuthenticationService.Login();

            var query = new GetPostReactionByIdQuery
            {
                PostId = Guid.NewGuid().ToString(),
                PostReactionId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_getting_post_reaction_by_id_when_post_reaction_is_not_exist()
        {
            var fakePost = await PostRepository.AsQuerable().PickRandom();

            AuthenticationService.Login();

            var query = new GetPostReactionByIdQuery
            {
                PostId = fakePost!.Id,
                PostReactionId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_getting_post_reaction_by_id_when_user_is_not_authorized()
        {
            var query = new GetPostReactionByIdQuery
            {
                PostId = Guid.NewGuid().ToString(),
                PostReactionId = Guid.NewGuid().ToString()
            };


            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
    }
}
