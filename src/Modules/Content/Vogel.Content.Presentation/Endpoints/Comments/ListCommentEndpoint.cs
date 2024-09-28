using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Application.Comments.Queries.ListComments;
using Vogel.Content.Presentation.Endpoints.Posts;

namespace Vogel.Content.Presentation.Endpoints.Comments
{
    public class ListCommentEndpoint : Endpoint<ListCommentsQuery , Paging<CommentDto>>
    {
        private readonly IMediator _mediator;

        public ListCommentEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{postId}/comments");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(Paging<CommentDto>))
                .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );
            Group<PostRoutingGroup>();
        }

        public override async Task HandleAsync(ListCommentsQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
