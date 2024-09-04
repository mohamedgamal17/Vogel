using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Application.Pictures.Queries.ListCurrentUserPictures;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Presentation.Endpoints.Pictures
{
    public class ListPictureEndpoint : Endpoint<ListCurrentUserPicturesQuery ,Paging<PictureDto>>
    {
        private readonly IMediator _mediator;

        public ListPictureEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(Paging<PictureDto>)));
            Group<PictureRoutingGroup>();

        }

        public override async Task HandleAsync(ListCurrentUserPicturesQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
