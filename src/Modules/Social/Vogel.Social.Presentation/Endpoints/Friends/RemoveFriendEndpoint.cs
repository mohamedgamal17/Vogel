using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Social.Application.Friendship.Commands.RemoveFriend;
namespace Vogel.Social.Presentation.Endpoints.Friends
{
    public class RemoveFriendEndpoint : Endpoint<RemoveFriendCommand>
    {
        private readonly IMediator _mediator;

        public RemoveFriendEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Delete("{id}");
            Description(b => b.Produces(StatusCodes.Status204NoContent)
               .ProducesProblem(StatusCodes.Status400BadRequest)
               .ProducesProblem(StatusCodes.Status404NotFound)
            );
            Group<FriendRoutingGroup>();
        }

        public override async Task HandleAsync(RemoveFriendCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToNoContent();

            await SendResultAsync(response);
        }
    }
}
