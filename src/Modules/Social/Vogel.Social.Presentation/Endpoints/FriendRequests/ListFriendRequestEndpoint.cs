using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Application.Friendship.Queries.ListFriendRequest;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Presentation.Endpoints.FriendRequests
{
    public class ListFriendRequestEndpoint : Endpoint<PagingParams, Paging<FriendRequestDto>>
    {
        private readonly IMediator _mediator;
        private readonly ISecurityContext _securityContext;
        public ListFriendRequestEndpoint(IMediator mediator, ISecurityContext securityContext)
        {
            _mediator = mediator;
            _securityContext = securityContext;
        }

        public override void Configure()
        {
            Get("");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(Paging<FriendRequestDto>)));
            Options(x => x.WithName("GetFriendRequestById"));
            Group<FriendRequestsRoutingGroup>();
        }

        public override async Task HandleAsync(PagingParams req, CancellationToken ct)
        {
            var query = new ListFriendRequestQuery
            {
                UserId = _securityContext.User!.Id,
                Cursor = req.Cursor,
                Asending = req.Asending,
                Limit = req.Limit
            };

            var result = await _mediator.Send(query);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
