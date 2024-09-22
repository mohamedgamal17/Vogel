using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Application.Friendship.Commands.RejectFriendRequest;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Presentation.Endpoints.FriendRequests
{
    public class RejcetFriendRequestEndpoint : Endpoint<RejectFriendRequestCommand, FriendRequestDto>
    {
        private readonly IMediator _mediator;

        public RejcetFriendRequestEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Post("{friendRequestId}/reject");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(FriendRequestDto))
               .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
               .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );
            Group<FriendRequestsRoutingGroup>();
        }

        public override async Task HandleAsync(RejectFriendRequestCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
