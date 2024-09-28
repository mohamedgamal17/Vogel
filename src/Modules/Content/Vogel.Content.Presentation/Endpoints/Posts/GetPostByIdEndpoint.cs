using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Application.Posts.Queries.GetPostbyId;
namespace Vogel.Content.Presentation.Endpoints.Posts
{
    public class GetPostByIdEndpoint : Endpoint<GetPostByIdQuery, PostDto>
    {
        private readonly IMediator _mediator;

        public GetPostByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{postId}");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(PostDto))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
              );

            Group<PostRoutingGroup>();
        }

        public override async Task HandleAsync(GetPostByIdQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
