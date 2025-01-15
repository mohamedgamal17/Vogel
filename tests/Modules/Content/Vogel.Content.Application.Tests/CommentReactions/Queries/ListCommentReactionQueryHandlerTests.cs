using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Content.Application.CommentReactions.Queries.ListCommentReactions;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Posts;

namespace Vogel.Content.Application.Tests.CommentReactions.Queries
{
    public class ListCommentReactionQueryHandlerTests : ContentTestFixture
    {
        public IContentRepository<Comment> CommentRepository { get;  }

        public IContentRepository<Post> PostRepository { get; }
        public ListCommentReactionQueryHandlerTests()
        {
            CommentRepository = ServiceProvider.GetRequiredService<IContentRepository<Comment>>();
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
        }

        [Test]
        public async Task Should_list_comment_reactions_by_comment_id()
        {
            AuthenticationService.Login();

            var fakeComment = await CommentRepository.AsQuerable().PickRandom();

            var query = new ListCommentReactionsQuery
            {
                CommentId = fakeComment!.Id,
                PostId = fakeComment!.PostId,
                Limit= 10
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.Count.Should().BeGreaterThan(0);

            result.Value!.Data.All(x => x.CommentId == fakeComment.Id).Should().BeTrue();
        }

        [Test]
        public async Task Should_failure_while_listing_comment_reactions_when_post_id_is_not_exist()
        {
            AuthenticationService.Login();

            var query = new ListCommentReactionsQuery
            {
                PostId = Guid.NewGuid().ToString(),
                CommentId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));

        }

        [Test]
        public async Task Should_failure_while_listing_comment_reactions_when_comment_id_is_not_exist()
        {
            AuthenticationService.Login();

            var fakePost = await PostRepository.AsQuerable().PickRandom();

            var query = new ListCommentReactionsQuery
            {
                PostId = fakePost!.Id,
                CommentId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_listing_comments_reaction_when_user_is_not_authorized()
        {
            var query = new ListCommentReactionsQuery()
            {
                PostId = Guid.NewGuid().ToString(),
                CommentId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
    }
}
