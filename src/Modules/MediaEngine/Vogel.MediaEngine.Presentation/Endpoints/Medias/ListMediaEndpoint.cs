using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.MediaEngine.Application.Medias.Queries.ListMedia;
using Vogel.MediaEngine.Shared.Dtos;

namespace Vogel.MediaEngine.Presentation.Endpoints.Medias
{
    public class ListMediaEndpoint : Endpoint<ListMediaQuery, Paging<MediaDto>>
    {
        private readonly IMediator _mediator;

        public ListMediaEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(Paging<MediaDto>)));
            Group<MediaRoutingGroup>();
        }

        public override async Task HandleAsync(ListMediaQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req, ct);
            await SendResultAsync(result.ToOk());
        }
    }
}
