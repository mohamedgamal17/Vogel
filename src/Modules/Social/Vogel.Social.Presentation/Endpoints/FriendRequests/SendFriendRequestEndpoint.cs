using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Social.Application.Friendship.Commands.SendFriendRequest;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Presentation.Endpoints.FriendRequests
{
    public class SendFriendRequestEndpoint : Endpoint<SendFriendRequestCommand,FriendRequestDto>
    {
        private readonly IMediator _mediator;

        public SendFriendRequestEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Post("");
            Description(x => x.Produces(StatusCodes.Status201Created, typeof(FriendRequestDto))
               .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
               );
            Group<FriendRequestsRoutingGroup>();
        }

        public override async Task HandleAsync(SendFriendRequestCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToCreatedAtRoute("GetFriendRequestById", new {friendRequestId = result.Value?.Id});

            await SendResultAsync(response);
        }
    }
}
