
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Application.PostReactions.Queries.GetPostReactionById;
using Vogel.Content.Presentation.Endpoints.Posts;

namespace Vogel.Content.Presentation.Endpoints.PostReactions
{
    public class GetCommentReactionByIdEndpoint : Endpoint<GetPostReactionByIdQuery, PostReactionDto>
    {
        private readonly IMediator _mediator;

        public GetCommentReactionByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{postId}/reactions/{reactionId}");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(PostReactionDto))
                .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );
            Options(x => x.WithName("GetPostReactionById"));
            Group<PostRoutingGroup>();
        }

        public override async Task HandleAsync(GetPostReactionByIdQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
