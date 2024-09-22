using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Application.Friendship.Queries.GetFriendRequestById;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Presentation.Endpoints.FriendRequests
{
    public class GetFriendRequestByIdEndpoint : Endpoint<GetFriendRequestByIdQuery, FriendRequestDto>
    {
        private readonly IMediator _mediator;

        public GetFriendRequestByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{friendRequestId}");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(FriendRequestDto))
                .Produces(StatusCodes.Status404NotFound,typeof(ProblemDetails))                                 
                );
            Group<FriendRequestsRoutingGroup>();
        }

        public override async Task HandleAsync(GetFriendRequestByIdQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
