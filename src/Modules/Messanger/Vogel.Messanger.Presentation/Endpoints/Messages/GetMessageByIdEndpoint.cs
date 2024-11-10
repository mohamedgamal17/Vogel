using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Messages.Queries.GetUserConversationMessage;

namespace Vogel.Messanger.Presentation.Endpoints.Messages
{
    public class GetMessageByIdEndpoint : Endpoint<GetUserConversationMessageQuery>
    {
        private readonly IMediator _mediator;

        public GetMessageByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{conversationId}/messages/{messageId}");
            Description(x => x.Produces(StatusCodes.Status201Created, typeof(ConversationDto))
             .Produces(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetails))
            );
            Options(x => x.WithName("GetMessageById"));
            Group<ConversationRoutingGroup>();
        }

        public override async Task HandleAsync(GetUserConversationMessageQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
