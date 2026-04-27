using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Vogel.MediaEngine.Presentation.Endpoints
{
    public class MediaEngineRoutingGroup : Group
    {
        public MediaEngineRoutingGroup()
        {
            Configure("media-engine", ep =>
            {
                ep.Description(x => x.Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
                    .Produces(StatusCodes.Status401Unauthorized, typeof(ProblemDetails))
                    .Produces(StatusCodes.Status403Forbidden, typeof(ProblemDetails))
                    .WithSummary("Media engine routing group"));
            });
        }
    }
}
