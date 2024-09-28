using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Application.Posts.Queries.ListUserPost;
namespace Vogel.Content.Presentation.Endpoints.People
{
    public class ListUserPostEndpoint :Endpoint<ListUserPostQuery , Paging<PostDto>>
    {
        private readonly IMediator _mediator;

        public ListUserPostEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{userId}/posts/");
            Description(ep => ep.Produces(StatusCodes.Status200OK, typeof(Paging<PostDto>)));
            Group<PeopleRoutingGroup>();

        }

        public override async Task HandleAsync(ListUserPostQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
