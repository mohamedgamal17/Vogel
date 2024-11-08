using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Vogel.Messanger.Presentation.Endpoints
{
    public class ConversationRoutingGroup : Group
    {
        public ConversationRoutingGroup()
        {
            Configure("conversations", ep =>
            {
                ep.Description(x => x.Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
                   .Produces(StatusCodes.Status401Unauthorized, typeof(ProblemDetails))
                   .Produces(StatusCodes.Status403Forbidden, typeof(ProblemDetails))
                   .WithSummary("Conversation routing grop"));
            });
        }
    }
}
