using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Application.Comments.Queries.GetCommentById;
using Vogel.Content.Application.Tests.Extensions;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Common;
using Vogel.Content.Domain.Posts;

namespace Vogel.Content.Application.Tests.Comments.Queries
{
    public class GetCommentQueryByIdHandlerTests : ContentTestFixture
    {
        public IContentRepository<Post> PostRepository { get;  }

        public IContentRepository<Comment> CommentRepository { get; }

        public IContentRepository<CommentReaction> CommentReactionRepository { get; }
        public GetCommentQueryByIdHandlerTests()
        {
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
            CommentRepository = ServiceProvider.GetRequiredService<IContentRepository<Comment>>();
            CommentReactionRepository = ServiceProvider.GetRequiredService<IContentRepository<CommentReaction>>();
        }


        [Test]
        public async Task Should_get_comment_by_id()
        {
            var fakeUser = UserService.PickRandomUser()!;
            var fakePost = await PostRepository.AsQuerable().PickRandom();
            var fakeComment = await CommentRepository.AsQuerable().Where(x => x.PostId == fakePost!.Id).PickRandom();
            var expectedReactionSummary = await CommentReactionRepository.AsQuerable().Where(x => x.CommentId == fakeComment!.Id)
                .GroupBy(x => x.CommentId)
                .Select(x => new CommentReactionSummaryDto
                {
                    Id = x.Key,
                    TotalAngry = x.Where(x => x.Type == ReactionType.Angry).LongCount(),
                    TotalLaugh = x.Where(x => x.Type == ReactionType.Laugh).LongCount(),
                    TotalLove = x.Where(x => x.Type == ReactionType.Love).LongCount(),
                    TotalLike = x.Where(x => x.Type == ReactionType.Like).LongCount(),
                    TotalSad = x.Where(x => x.Type == ReactionType.Sad).LongCount()
                }).FirstAsync();

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new GetCommentByIdQuery
            {
                PostId = fakePost!.Id,
                CommentId = fakeComment!.Id
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertCommentDto(fakeComment);

            result.Value!.AssertReactionSummary(expectedReactionSummary);

        }

        [Test]
        public async Task Should_failure_while_getting_comment_by_id_when_post_is_not_exist()
        {
            AuthenticationService.Login();

            var query = new GetCommentByIdQuery
            {
                PostId = Guid.NewGuid().ToString(),
                CommentId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_getting_comment_by_id_when_comment_id_is_not_exist()
        {
            var fakePost = await PostRepository.AsQuerable().PickRandom();

            AuthenticationService.Login();

            var query = new GetCommentByIdQuery
            {
                PostId = fakePost!.Id,
                CommentId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_getting_comment_by_id_when_user_is_not_exist()
        {
            var query = new GetCommentByIdQuery
            {
                PostId = Guid.NewGuid().ToString(),
                CommentId = Guid.NewGuid().ToString()
            };


            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

    }
}
