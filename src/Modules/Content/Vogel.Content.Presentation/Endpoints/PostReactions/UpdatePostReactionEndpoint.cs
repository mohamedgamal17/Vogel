using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Application.PostReactions.Commands.UpdatePostReaction;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Presentation.Endpoints.Posts;
namespace Vogel.Content.Presentation.Endpoints.PostReactions
{
    public class UpdatePostReactionEndpoint : Endpoint<UpdatePostReactionCommand, PostReactionDto>
    {
        private readonly IMediator _mediator;

        public UpdatePostReactionEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Put("{postId}/reactions/{reactionId}");
            Description(x => x.Produces(StatusCodes.Status201Created, typeof(CommentDto))
                .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );
            Group<PostRoutingGroup>();
        }

        public override async Task HandleAsync(UpdatePostReactionCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
