using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Application.Posts.Queries.GetPostbyId;
using Vogel.Content.Application.Tests.Extensions;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Common;
using Vogel.Content.Domain.Posts;

namespace Vogel.Content.Application.Tests.Posts.Queries
{
    public class GetPostByIdQueryHandlerTests : ContentTestFixture
    {
        public IContentRepository<Post> PostRepository { get; }
        public IContentRepository<PostReaction> PostReactionRepository { get; }
        public GetPostByIdQueryHandlerTests()
        {
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
            PostReactionRepository = ServiceProvider.GetRequiredService<IContentRepository<PostReaction>>();
        }

        [Test]
        public async Task Should_get_post_by_id()
        {
            AuthenticationService.Login();

            var fakePost = await PostRepository.AsQuerable().PickRandom();

            var expectedReactionSummary = await PostReactionRepository.AsQuerable()
                .Where(x => x.PostId == fakePost!.Id)
                .GroupBy(x => x.PostId)
                .Select(x => new PostReactionSummaryDto
                {
                    Id = x.Key,
                    TotalAngry = x.Where(x => x.Type == ReactionType.Angry).LongCount(),
                    TotalLaugh = x.Where(x => x.Type == ReactionType.Laugh).LongCount(),
                    TotalLove = x.Where(x => x.Type == ReactionType.Love).LongCount(),
                    TotalLike = x.Where(x => x.Type == ReactionType.Like).LongCount(),
                    TotalSad = x.Where(x => x.Type == ReactionType.Sad).LongCount()
                }).FirstAsync();


            var query = new GetPostByIdQuery
            {
                PostId = fakePost!.Id
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertPostDto(fakePost);

            result.Value!.AssertReactionSummaryDto(expectedReactionSummary);
        }

        [Test]
        public async Task Should_failure_while_getting_post_by_id_when_post_id_is_not_exit()
        {
            AuthenticationService.Login();

            var query = new GetPostByIdQuery
            {
                PostId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_getting_post_by_id_when_user_is_not_authorized()
        {
            var query = new GetPostByIdQuery
            {
                PostId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
    }
}
