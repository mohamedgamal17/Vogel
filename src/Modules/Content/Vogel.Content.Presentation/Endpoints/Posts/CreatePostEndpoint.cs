using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Content.Application.Posts.Commands.CreatePost;
using Vogel.Content.Application.Posts.Dtos;
namespace Vogel.Content.Presentation.Endpoints.Posts
{
    public class CreatePostEndpoint : Endpoint<CreatePostCommand,PostDto>
    {
        private readonly IMediator _mediator;

        public CreatePostEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Post("");
            Description(x => x.Produces(StatusCodes.Status201Created, typeof(PostDto))
                .Produces(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetails))
            );
            Group<PostRoutingGroup>();
        }

        public override async Task HandleAsync(CreatePostCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToCreatedAtRoute("GetPostById", new {postId = result.Value?.Id});

            await SendResultAsync(response);
        }
    }
}
