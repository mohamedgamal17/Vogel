using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Social.Application.Users.Commands.UpdateUser;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Presentation.Endpoints.Users
{
    public class UpdateUserProfileEndpoint : Endpoint<UpdateUserCommand, UserDto>
    {
        private readonly IMediator _mediator;

        public UpdateUserProfileEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Put("");
            Description(x => x.Produces(StatusCodes.Status201Created, typeof(UserDto))
                .Produces(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetails))
            );
            Group<UserRoutingGroup>();
        }

        public override async Task HandleAsync(UpdateUserCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
