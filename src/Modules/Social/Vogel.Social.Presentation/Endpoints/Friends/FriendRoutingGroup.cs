using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Vogel.Social.Presentation.Endpoints.Friends
{
    public class FriendRoutingGroup : Group
    {
        public FriendRoutingGroup()
        {
            Configure("friends", ep =>
            {
                ep.Description(x =>
                {
                    x.Produces(401, typeof(ProblemDetails))
                    .Produces(500, typeof(ProblemDetails))
                    .WithDescription("user friends routes");

                });
            });
        }
    }
}
