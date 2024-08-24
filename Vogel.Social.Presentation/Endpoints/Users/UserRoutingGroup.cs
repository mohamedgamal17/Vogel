using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Vogel.Social.Presentation.Endpoints.Users
{
    public class UserRoutingGroup : Group
    {
        public UserRoutingGroup()
        {
            Configure("user", ep =>
            {
                ep.Description(x =>
                {
                    x.Produces(401, typeof(ProblemDetails))
                    .Produces(500, typeof(ProblemDetails))
                    .WithTags("users")
                    .WithSummary("user profile routes");
                });
            });
        }
    }
}
