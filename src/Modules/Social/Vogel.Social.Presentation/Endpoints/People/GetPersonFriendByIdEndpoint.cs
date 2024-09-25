using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Social.Application.Friendship.Queries.GetFriendRequestById;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Presentation.Endpoints.People
{
    public class GetPersonFriendByIdEndpoint : Endpoint<GetFriendRequestByIdQuery, FriendDto>
    {
        private readonly IMediator _mediator;

        public GetPersonFriendByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{userId}/friends/{friendRequestId}");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(FriendDto))
                .ProducesProblem(StatusCodes.Status404NotFound)
               );

            Group<PeopleRoutingGroup>();
        }

        public override async Task HandleAsync(GetFriendRequestByIdQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
