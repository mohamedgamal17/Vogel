using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Content.Application.Posts.Commands.UpdatePost;
using Vogel.Content.Application.Posts.Dtos;

namespace Vogel.Content.Presentation.Endpoints.Posts
{
    public class UpdatePostEndpoint : Endpoint<UpdatePostCommand ,PostDto>
    {
        private readonly IMediator _mediator;

        public UpdatePostEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Put("{postId}");
            Description(x => x.Produces(StatusCodes.Status201Created, typeof(PostDto))
              .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
              .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
          );
            Group<PostRoutingGroup>();
        }

        public override async Task HandleAsync(UpdatePostCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
