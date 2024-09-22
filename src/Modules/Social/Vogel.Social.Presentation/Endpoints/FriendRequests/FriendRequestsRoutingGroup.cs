using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Vogel.Social.Presentation.Endpoints.FriendRequests
{
    public class FriendRequestsRoutingGroup : Group
    {
        public FriendRequestsRoutingGroup()
        {
            Configure("friendRequests", ep =>
            {
                ep.Description(x => x.Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
                  .WithSummary("people routes")
              );
            });
        }
    }
}
