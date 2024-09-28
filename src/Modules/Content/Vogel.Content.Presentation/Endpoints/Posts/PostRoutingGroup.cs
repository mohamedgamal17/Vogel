using FastEndpoints;
using Microsoft.AspNetCore.Http;
namespace Vogel.Content.Presentation.Endpoints.Posts
{
    public class PostRoutingGroup : Group
    {
        public PostRoutingGroup()
        {
            Configure("posts", ep =>
            {
                ep.Description(x => x.Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
                    .Produces(StatusCodes.Status401Unauthorized, typeof(ProblemDetails))
                    .Produces(StatusCodes.Status403Forbidden, typeof(ProblemDetails))
                    .WithSummary("posts routing")
                );
            });
        }
    }
}
