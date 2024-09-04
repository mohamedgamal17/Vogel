using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Social.Application.Users.Queries.GetCurrentUser;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Presentation.Endpoints.Users
{
    public class GetUserProfileEndpoint : Endpoint<EmptyRequest,UserDto>
    {
        private readonly IMediator _mediator;

        public GetUserProfileEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("");
            Options(x => x.WithName("GetCurrentUserProfile"));
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(UserDto))
                .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
            );
            Group<UserRoutingGroup>();
        }


        public override async Task HandleAsync(EmptyRequest req ,CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCurrentUserQuery());

            var response = result.ToOk();

            await SendResultAsync(response);
        }
      
    }
}
