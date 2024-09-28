using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Application.PostReactions.Queries.ListPostReactions;
using Vogel.Content.Presentation.Endpoints.Posts;

namespace Vogel.Content.Presentation.Endpoints.CommentsReactions
{
    public class ListCommentReactionEndpoint : Endpoint<ListPostReactionsQuery, Paging<CommentReactionDto>>
    {
        private readonly IMediator _mediator;

        public ListCommentReactionEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{postId}/comments/{commentId}/reactions");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(Paging<CommentReactionDto>))
                .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );
            Group<PostRoutingGroup>();
        }


        public override async Task HandleAsync(ListPostReactionsQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
