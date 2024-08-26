using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Vogel.Social.Presentation.Endpoints.Pictures
{
    internal class PictureRoutingGroup : Group
    {

        public PictureRoutingGroup()
        {
            Configure("pictures", ep =>
            {
                ep.Description(x =>
                {
                    x.Produces(401, typeof(ProblemDetails))
                    .Produces(500, typeof(ProblemDetails))
                    .WithDescription("user pictures routes");

                });
            });
        }
    }
}
