using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Application.Friendship.Queries.ListFriendRequest;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Presentation.Endpoints.Friends
{
    public class ListUserFriendsEndpoint : Endpoint<PagingParams, Paging<FriendDto>>
    {
        private readonly IMediator _mediator;
        private readonly ISecurityContext _securityContext;

        public ListUserFriendsEndpoint(IMediator mediator, ISecurityContext securityContext)
        {
            _mediator = mediator;
            _securityContext = securityContext;
        }

        public override void Configure()
        {
            Get("");
            Description(x => x.Produces(StatusCodes.Status200OK , typeof(Paging<FriendDto>)));
            Group<FriendRoutingGroup>();
        }

        public override async Task HandleAsync(PagingParams req, CancellationToken ct)
        {
            var query = new ListFriendRequestQuery
            {
                Asending = req.Asending,
                Cursor = req.Cursor,
                Limit = req.Limit,
                UserId = _securityContext.User!.Id
            };

            var result = await _mediator.Send(query);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
