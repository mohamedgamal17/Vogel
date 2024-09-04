using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Social.Application.Pictures.Queries.GetPictureById;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Presentation.Endpoints.Pictures
{
    public class GetPictureByIdEndpoint : Endpoint<GetPictureByIdQuery , PictureDto>
    {
        private readonly IMediator _mediator;

        public GetPictureByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{id}");
            Options(x => x.WithName("GetPictureById"));
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(PictureDto))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );
            Group<PictureRoutingGroup>();
        }

        public override async Task HandleAsync(GetPictureByIdQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
