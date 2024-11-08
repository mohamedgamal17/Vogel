using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Conversations.Queries.GetUserConversationParticipant;
namespace Vogel.Messanger.Presentation.Endpoints.Participants
{
    public class GetParticipantByIdEndpoint : Endpoint<GetUserConversationParticipantQuery>
    {
        private readonly IMediator _mediator;
        public GetParticipantByIdEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override void Configure()
        {
            Get("{conversationId}/participants/{participantId}");
            Description(x => x.Produces(StatusCodes.Status200OK, typeof(ParticipantDto)));
            Group<ConversationRoutingGroup>();
        }

        public override async Task HandleAsync(GetUserConversationParticipantQuery req, CancellationToken ct)
        {
            var result = await _mediator.Send(req);

            var response = result.ToOk();

            await SendResultAsync(response);
        }
    }
}
