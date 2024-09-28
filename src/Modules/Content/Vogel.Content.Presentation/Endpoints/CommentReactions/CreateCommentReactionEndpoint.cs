using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Content.Application.CommentReactions.Commands.CreateCommentReaction;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Presentation.Endpoints.Posts;
namespace Vogel.Content.Presentation.Endpoints.CommentsReactions
{
    public class CreateCommentReactionEndpoint : Endpoint<CreateCommentReactionCommand , CommentReactionDto>
    {
        private readonly IMediator _mediator;

        public CreateCommentReactionEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Post("{postId}/comments/{commentId}/reactions");
            Description(x => x.Produces(StatusCodes.Status201Created, typeof(CommentReactionDto))
                .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );
            Group<PostRoutingGroup>();
        }


        public override async Task HandleAsync(CreateCommentReactionCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToCreatedAtRoute("GetCommentReactionById", new { postId = req.PostId, commentId = req.CommentId, reactionId = result.Value?.Id });

            await SendResultAsync(response);
        }
    }
}
