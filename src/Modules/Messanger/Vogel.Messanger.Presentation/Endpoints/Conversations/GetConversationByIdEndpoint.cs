using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Conversations.Queries.GetUserConversationById;

namespace Vogel.Messanger.Presentation.Endpoints.Conversations
{
    public class GetConversationByIdEndpoint : Endpoint<GetConversationByIdQuery>
    {
        private readonly IMediator _mediator;

        public GetConversationByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{conversationId}");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(ConversationDto))
             .Produces(StatusCodes.Status404NotFound, typeof(ValidationProblemDetails))
            );
            Options(x => x.WithName("GetConversationById"));
            Group<ConversationRoutingGroup>();
        }
        public override async Task HandleAsync(GetConversationByIdQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response =  result.ToOk();

            await SendResultAsync(response);
        }
    }
}
