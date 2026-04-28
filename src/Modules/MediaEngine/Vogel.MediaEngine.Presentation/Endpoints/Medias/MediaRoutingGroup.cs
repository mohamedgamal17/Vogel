using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Vogel.MediaEngine.Presentation.Endpoints.Medias
{
    internal class MediaRoutingGroup : Group
    {
        public MediaRoutingGroup()
        {
            Configure("medias", ep =>
            {
                ep.Description(x =>
                {
                    x.Produces(StatusCodes.Status401Unauthorized, typeof(ProblemDetails))
                     .Produces(StatusCodes.Status403Forbidden, typeof(ProblemDetails))
                     .Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
                     .WithDescription("Media engine routes");
                });
            });
        }
    }
}
