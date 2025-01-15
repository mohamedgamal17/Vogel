using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Content.Application.CommentReactions.Queries.GetCommentReactionById;
using Vogel.Content.Application.Tests.Extensions;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Posts;

namespace Vogel.Content.Application.Tests.CommentReactions.Queries
{
    public class GetCommentReactionByIdQueryHandlerTests : ContentTestFixture
    {
        public IContentRepository<Post> PostRepository { get; }
        public IContentRepository<Comment> CommentRepository { get; }
        public IContentRepository<CommentReaction> CommentReactionRepository { get;  }

        public GetCommentReactionByIdQueryHandlerTests()
        {
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
            CommentRepository = ServiceProvider.GetRequiredService<IContentRepository<Comment>>();
            CommentReactionRepository = ServiceProvider.GetRequiredService<IContentRepository<CommentReaction>>();
        }

        [Test]
        public async Task Should_get_comment_reaction_by_id()
        {
            AuthenticationService.Login();

            var fakeComment = await CommentRepository.AsQuerable().PickRandom();

            var fakeCommentReaction = await CommentReactionRepository.AsQuerable().Where(x => x.CommentId == fakeComment!.Id).PickRandom();

            var query = new GetCommentReactionByIdQuery
            {
                PostId = fakeComment!.PostId,
                CommentId = fakeComment!.Id,
                ReactionId = fakeCommentReaction!.Id
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertCommentReactionDto(fakeCommentReaction);
        }

        [Test]
        public async Task Should_failure_while_getting_comment_reaction_by_id_when_post_id_is_not_exist()
        {
            AuthenticationService.Login();

            var fakeComment = await CommentRepository.AsQuerable().PickRandom();

            var fakeCommentReaction = await CommentReactionRepository.AsQuerable().Where(x => x.CommentId == fakeComment!.Id).PickRandom();

            var query = new GetCommentReactionByIdQuery
            {
                PostId = Guid.NewGuid().ToString(),
                CommentId = fakeComment!.Id,
                ReactionId = fakeCommentReaction!.Id
            };


            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }



        [Test]
        public async Task Should_failure_while_getting_comment_reaction_by_id_when_comment_id_is_not_exist()
        {
            AuthenticationService.Login();

            var fakePost=  await PostRepository.AsQuerable().PickRandom();

            var fakeCommentReaction = await CommentReactionRepository.AsQuerable().PickRandom();

            var query = new GetCommentReactionByIdQuery
            {
                PostId = fakePost!.Id,
                CommentId = Guid.NewGuid().ToString(),
                ReactionId = fakeCommentReaction!.Id
            };


            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_getting_comment_reaction_by_id_when_comment_reaction_id_is_not_exist()
        {
            AuthenticationService.Login();

            var fakeComment = await CommentRepository.AsQuerable().PickRandom();

            var query = new GetCommentReactionByIdQuery
            {
                PostId = fakeComment!.PostId,
                CommentId = fakeComment!.Id,
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }


        [Test]
        public async Task Should_failure_while_getting_comment_reaction_by_id_when_user_is_not_authorized()
        {

            var fakeComment = await CommentRepository.AsQuerable().PickRandom();

            var fakeCommentReaction = await CommentReactionRepository.AsQuerable().Where(x => x.CommentId == fakeComment!.Id).PickRandom();

            var query = new GetCommentReactionByIdQuery
            {
                PostId = fakeComment!.PostId,
                CommentId = fakeComment!.Id,
                ReactionId = fakeCommentReaction!.Id
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }

    }
}
