using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Application.Friendship.Commands.AcceptFriendRequest;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Presentation.Endpoints.FriendRequests
{
    public class AcceptFriendRequestEndpoint : Endpoint<AcceptFriendRequestCommand,FriendDto>
    {
        private readonly IMediator _mediator;

        public AcceptFriendRequestEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Post("{friendRequestId}/accept");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(FriendDto))
                .Produces(StatusCodes.Status400BadRequest,typeof(ProblemDetails))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );

            Group<FriendRequestsRoutingGroup>();
        }

        public override async Task HandleAsync(AcceptFriendRequestCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
