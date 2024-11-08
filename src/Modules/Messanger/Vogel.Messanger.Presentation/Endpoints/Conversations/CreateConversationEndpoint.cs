using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Messanger.Application.Conversations.Commands.CreateConversation;
using Vogel.Messanger.Application.Conversations.Dtos;

namespace Vogel.Messanger.Presentation.Endpoints.Conversations
{
    public class CreateConversationEndpoint : Endpoint<CreateConversationCommand>
    {
        private readonly IMediator _mediator;

        public CreateConversationEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Post("");
            Description(x => x.Produces(StatusCodes.Status201Created, typeof(ConversationDto))
                .Produces(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetails))
            );
            Group<ConversationRoutingGroup>();
        }


        public override async Task HandleAsync(CreateConversationCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToCreatedAtRoute("GetConversationById", new {conversationId = result.Value?.Id});

            await SendResultAsync(response);
        }
    }
}
