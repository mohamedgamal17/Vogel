using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Conversations.Queries.ListUserConversationParticipant;

namespace Vogel.Messanger.Presentation.Endpoints.Participants
{
    public class ListParticipantEndpoint : Endpoint<ListUserConversationParticipantQuery>
    {
        private readonly IMediator _mediator;

        public ListParticipantEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{conversationId}/participants");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(Paging<ParticipantDto>)));
            Group<ConversationRoutingGroup>();
        }

        public override async Task HandleAsync(ListUserConversationParticipantQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
