using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Application.Posts.Queries.ListPost;
namespace Vogel.Content.Presentation.Endpoints.Posts
{
    public class ListPostEndpoint : Endpoint<ListPostQuery , Paging<PostDto>>
    {
        private readonly IMediator _mediator;

        public ListPostEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(Paging<PostDto>)));
            Group<PostRoutingGroup>();
        }

        public override async Task HandleAsync(ListPostQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
