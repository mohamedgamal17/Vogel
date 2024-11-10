using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Messages.Queries.ListUserConversationMessage;

namespace Vogel.Messanger.Presentation.Endpoints.Messages
{
    public class ListMessageEndpoint : Endpoint<ListUserConversationMessageQuery>
    {
        private readonly IMediator _mediator;

        public ListMessageEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{conversationId}/messages");
            Description(x => x.Produces(StatusCodes.Status201Created, typeof(ConversationDto))
             .Produces(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetails))
            );
            Group<ConversationRoutingGroup>();
        }

        public override async Task HandleAsync(ListUserConversationMessageQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
