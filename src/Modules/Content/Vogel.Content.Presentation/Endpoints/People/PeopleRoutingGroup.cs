
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Vogel.Content.Presentation.Endpoints.People
{
    public class PeopleRoutingGroup : Group
    {
        public PeopleRoutingGroup()
        {
            Configure("people", ep =>
            {
                ep.Description(x => x.Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
                );
            });
        }
    }
}
