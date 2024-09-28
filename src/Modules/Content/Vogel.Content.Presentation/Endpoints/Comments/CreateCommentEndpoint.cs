using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Content.Application.Comments.Commands.CreateComment;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Presentation.Endpoints.Posts;

namespace Vogel.Content.Presentation.Endpoints.Comments
{
    public class CreateCommentEndpoint : Endpoint<CreateCommentCommand ,CommentDto>
    {
        private readonly IMediator _mediator;

        public CreateCommentEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Post("{postId}/comments");
            Description(x => x.Produces(StatusCodes.Status201Created, typeof(CommentDto))
                .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );
            Group<PostRoutingGroup>();
        }

        public override async Task HandleAsync(CreateCommentCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToCreatedAtRoute("GetCommentById", new { postId = result.Value?.PostId, commentId = result.Value?.Id });

            await SendResultAsync(response);
        }
    }
}
