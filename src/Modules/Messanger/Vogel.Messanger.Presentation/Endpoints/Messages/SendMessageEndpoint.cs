using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Messages.Commands.SendMessage;

namespace Vogel.Messanger.Presentation.Endpoints.Messages
{
    public class SendMessageEndpoint : Endpoint<SendMessageCommand>
    {
        private readonly IMediator _mediator;

        public SendMessageEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Post("{conversationId}/messages");
            Description(x => x.Produces(StatusCodes.Status201Created, typeof(ConversationDto))
               .Produces(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetails))
           );
            Group<ConversationRoutingGroup>();
        }


        public override async Task HandleAsync(SendMessageCommand req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToCreatedAtRoute("GetMessageById", new { conversationId = result.Value?.ConversationId, messageId = result.Value?.Id });

            await SendResultAsync(response);
        }
    }
}
