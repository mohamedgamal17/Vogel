using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Application.Friendship.Queries.ListFriends;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Presentation.Endpoints.People
{
    public class ListPersonFriendsEndpoint : Endpoint<ListFriendsQuery, Paging<FriendDto>>
    {
        private readonly IMediator _mediator;

        public ListPersonFriendsEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{userId}/friends");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(Paging<FriendDto>)));
            Group<PeopleRoutingGroup>();
        }

        public override async Task HandleAsync(ListFriendsQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
