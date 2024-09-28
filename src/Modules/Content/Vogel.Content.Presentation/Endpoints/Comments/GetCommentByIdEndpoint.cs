using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Application.Comments.Queries.GetCommentById;
using Vogel.Content.Presentation.Endpoints.Posts;

namespace Vogel.Content.Presentation.Endpoints.Comments
{
    public class GetCommentByIdEndpoint : Endpoint<GetCommentByIdQuery , CommentDto>
    {
        private readonly IMediator _mediator;

        public GetCommentByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{postId}/comments/{commentId}");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(CommentDto))
                .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );
            Options(x => x.WithName("GetCommentById"));
            Group<PostRoutingGroup>();
        }

        public override async Task HandleAsync(GetCommentByIdQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
