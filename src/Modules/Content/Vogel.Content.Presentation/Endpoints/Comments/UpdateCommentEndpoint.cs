using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Content.Application.Comments.Commands.UpdateComment;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Presentation.Endpoints.Posts;

namespace Vogel.Content.Presentation.Endpoints.Comments
{
    public class UpdateCommentEndpoint :Endpoint<UpdateCommentCommand , CommentDto>
    {
        private readonly IMediator _mediator;

        public UpdateCommentEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override void Configure()
        {
            Put("{postId}/comments/{commentId}");

            Description(x => x.Produces(StatusCodes.Status200OK, typeof(CommentDto))
                .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );

            Group<PostRoutingGroup>();
        }

        public override async Task HandleAsync(UpdateCommentCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
