using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Application.Friendship.Queries.GetFriendById;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Presentation.Endpoints.Friends
{
    public class GetUserFriendByIdEndpoint : Endpoint<GetFriendByIdQuery, FriendDto>
    {
        private readonly IMediator _mediator;

        public GetUserFriendByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{friendId}");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(Paging<FriendDto>)));
            Group<FriendRoutingGroup>();
        }

        public override async Task HandleAsync(GetFriendByIdQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
