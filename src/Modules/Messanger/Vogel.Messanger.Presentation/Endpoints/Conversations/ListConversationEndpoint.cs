using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Conversations.Queries.ListUserConversation;

namespace Vogel.Messanger.Presentation.Endpoints.Conversations
{
    public class ListConversationEndpoint : Endpoint<ListUserConversationQuery>
    {
        private readonly IMediator _mediator;

        public ListConversationEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(Paging<ConversationDto>)));
            Group<ConversationRoutingGroup>();
        }

        public override async Task HandleAsync(ListUserConversationQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await  SendResultAsync(response);
        }
    }
}
