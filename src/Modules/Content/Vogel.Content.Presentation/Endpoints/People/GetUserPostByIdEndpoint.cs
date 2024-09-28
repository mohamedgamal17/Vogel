using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Application.Posts.Queries.GetUserPostById;
namespace Vogel.Content.Presentation.Endpoints.People
{
    public class GetUserPostByIdEndpoint : Endpoint<GetUserPostByIdQuery, PostDto>
    {
        private readonly IMediator _mediator;

        public GetUserPostByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{userId}/posts/{postId}");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(PostDto))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );
            Group<PeopleRoutingGroup>();
        }

        public override async Task HandleAsync(GetUserPostByIdQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
