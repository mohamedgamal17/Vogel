using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Application.Users.Queries.ListUsers;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Presentation.Endpoints.People
{
    public class ListUsersEndpoint : Endpoint<ListUsersQuery , Paging<UserDto>>
    {
        private readonly IMediator _mediator;

        public ListUsersEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(Paging<UserDto>)));
            Group<PeopleRoutingGroup>();
        }

        public override async Task HandleAsync(ListUsersQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
