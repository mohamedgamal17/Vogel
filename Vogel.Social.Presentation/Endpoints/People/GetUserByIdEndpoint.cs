using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Social.Application.Users.Queries.GetUserById;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Presentation.Endpoints.People
{
    public class GetUserByIdEndpoint : Endpoint<GetUserByIdQuery, UserDto>
    {
        private readonly IMediator _mediator;

        public GetUserByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{id}");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(UserDto))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );
            Group<PeopleRoutingGroup>();
        }

        public override async Task HandleAsync(GetUserByIdQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);
            var response = result.ToOk();
            await SendResultAsync(response);
        }
    }
}
