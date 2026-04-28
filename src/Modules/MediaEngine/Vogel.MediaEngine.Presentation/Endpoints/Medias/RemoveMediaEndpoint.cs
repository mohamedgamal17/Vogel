using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.MediaEngine.Application.Medias.Commands.RemoveMedia;

namespace Vogel.MediaEngine.Presentation.Endpoints.Medias
{
    public class RemoveMediaEndpoint : Endpoint<RemoveMediaCommand>
    {
        private readonly IMediator _mediator;

        public RemoveMediaEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Delete("{id}");
            Description(x => x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status404NotFound));
            Group<MediaRoutingGroup>();
        }

        public override async Task HandleAsync(RemoveMediaCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req, ct);
            await SendResultAsync(result.ToNoContent());
        }
    }
}
