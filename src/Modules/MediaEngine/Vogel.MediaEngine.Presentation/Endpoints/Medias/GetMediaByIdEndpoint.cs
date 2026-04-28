using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.MediaEngine.Application.Medias.Queries.GetMediaById;
using Vogel.MediaEngine.Shared.Dtos;

namespace Vogel.MediaEngine.Presentation.Endpoints.Medias
{
    public class GetMediaByIdEndpoint : Endpoint<GetMediaByIdQuery, MediaDto>
    {
        private readonly IMediator _mediator;

        public GetMediaByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{mediaId}");
            Options(x => x.WithName("GetMediaById"));
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(MediaDto))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails)));
            Group<MediaRoutingGroup>();
        }

        public override async Task HandleAsync(GetMediaByIdQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req, ct);
            await SendResultAsync(result.ToOk());
        }
    }
}
