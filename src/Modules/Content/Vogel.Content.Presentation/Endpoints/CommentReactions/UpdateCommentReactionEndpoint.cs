using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Content.Application.CommentReactions.Commands.UpdateCommentReaction;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Presentation.Endpoints.Posts;
namespace Vogel.Content.Presentation.Endpoints.CommentsReactions
{
    public class UpdateCommentReactionEndpoint : Endpoint<UpdateCommentReactionCommand, CommentReactionDto>
    {
        private readonly IMediator _mediator;

        public UpdateCommentReactionEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Put("{postId}/comments/{commentId}/reactions/{reactionId}");
            Description(x => x.Produces(StatusCodes.Status201Created, typeof(CommentReactionDto))
                .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );
            Group<PostRoutingGroup>();
        }

        public override async Task HandleAsync(UpdateCommentReactionCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
